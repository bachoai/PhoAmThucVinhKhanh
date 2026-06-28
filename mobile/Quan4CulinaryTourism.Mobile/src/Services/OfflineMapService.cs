using System.IO.Compression;
using System.Text.Json;
using Microsoft.Maui.Devices.Sensors;
using Quan4CulinaryTourism.Mobile.Models;

namespace Quan4CulinaryTourism.Mobile.Services;

public sealed class OfflineMapService
{
    private const string DefaultEntryFile = "index.html";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly OfflineDatabaseService _offlineDatabaseService;

    public OfflineMapService(OfflineDatabaseService offlineDatabaseService)
    {
        _offlineDatabaseService = offlineDatabaseService;
    }

    public async Task<MapPackResponse?> PrepareRenderablePackAsync(
        MapPackResponse? mapPack,
        IReadOnlyCollection<PoiResponse> pois,
        Location? userLocation,
        CancellationToken cancellationToken = default)
    {
        await _offlineDatabaseService.InitializeAsync();

        var workingPack = mapPack ?? new MapPackResponse
        {
            Version = "runtime",
            Name = "Offline Runtime Map",
            EntryFile = DefaultEntryFile,
            IsActive = true
        };

        var templatePath = await EnsureTemplatePathAsync(workingPack, cancellationToken);
        if (string.IsNullOrWhiteSpace(templatePath) || !File.Exists(templatePath))
        {
            return null;
        }

        workingPack.LocalEntryHtmlPath = await RenderHtmlAsync(workingPack, templatePath, pois, userLocation, cancellationToken);
        workingPack.DownloadedAtUtc ??= DateTime.UtcNow;

        await _offlineDatabaseService.SaveMapPackAsync(workingPack);
        return workingPack;
    }

    private async Task<string> EnsureTemplatePathAsync(MapPackResponse mapPack, CancellationToken cancellationToken)
    {
        var entryFile = string.IsNullOrWhiteSpace(mapPack.EntryFile) ? DefaultEntryFile : mapPack.EntryFile;

        if (!string.IsNullOrWhiteSpace(mapPack.LocalPackagePath) && File.Exists(mapPack.LocalPackagePath))
        {
            var extension = Path.GetExtension(mapPack.LocalPackagePath);
            if (extension.Equals(".zip", StringComparison.OrdinalIgnoreCase))
            {
                var extractFolder = Path.Combine(FileSystem.AppDataDirectory, "maps", $"pack-{Sanitize(mapPack.Version)}");
                var entryPath = Path.Combine(extractFolder, entryFile);

                if (!File.Exists(entryPath))
                {
                    if (Directory.Exists(extractFolder))
                    {
                        Directory.Delete(extractFolder, true);
                    }

                    Directory.CreateDirectory(extractFolder);
                    ZipFile.ExtractToDirectory(mapPack.LocalPackagePath, extractFolder, true);
                }

                mapPack.ExtractedDirectoryPath = extractFolder;
                if (File.Exists(entryPath))
                {
                    return entryPath;
                }
            }
            else if (extension.Equals(".html", StringComparison.OrdinalIgnoreCase) || extension.Equals(".htm", StringComparison.OrdinalIgnoreCase))
            {
                return mapPack.LocalPackagePath;
            }
        }

        return await EnsureFallbackTemplateAsync(cancellationToken);
    }

    private async Task<string> RenderHtmlAsync(
        MapPackResponse mapPack,
        string templatePath,
        IReadOnlyCollection<PoiResponse> pois,
        Location? userLocation,
        CancellationToken cancellationToken)
    {
        var template = await File.ReadAllTextAsync(templatePath, cancellationToken);
        if (!template.Contains("__POI_DATA__", StringComparison.Ordinal) &&
            !template.Contains("__USER_LOCATION__", StringComparison.Ordinal) &&
            !template.Contains("__PACK_NAME__", StringComparison.Ordinal))
        {
            return templatePath;
        }

        var renderedFolder = Path.Combine(FileSystem.AppDataDirectory, "maps", "rendered");
        Directory.CreateDirectory(renderedFolder);

        var renderedHtml = template
            .Replace("__PACK_NAME__", System.Net.WebUtility.HtmlEncode(mapPack.Name), StringComparison.Ordinal)
            .Replace("__POI_DATA__", SerializePois(pois), StringComparison.Ordinal)
            .Replace("__USER_LOCATION__", SerializeUserLocation(userLocation), StringComparison.Ordinal);

        var renderedPath = Path.Combine(renderedFolder, $"offline-map-{Sanitize(mapPack.Version)}.html");
        await File.WriteAllTextAsync(renderedPath, renderedHtml, cancellationToken);
        return renderedPath;
    }

    private static string SerializePois(IReadOnlyCollection<PoiResponse> pois)
    {
        var payload = pois
            .Where(static poi => poi.Latitude != 0 && poi.Longitude != 0)
            .OrderByDescending(static poi => poi.Priority)
            .ThenBy(static poi => poi.Name)
            .Select(poi => new
            {
                poi.Id,
                poi.Name,
                poi.Description,
                poi.Latitude,
                poi.Longitude,
                poi.Priority,
                DisplayAddress = poi.DisplayAddress
            })
            .ToList();

        return JsonSerializer.Serialize(payload, JsonOptions);
    }

    private static string SerializeUserLocation(Location? userLocation) =>
        userLocation is null
            ? "null"
            : JsonSerializer.Serialize(new
            {
                userLocation.Latitude,
                userLocation.Longitude
            }, JsonOptions);

    private static async Task<string> EnsureFallbackTemplateAsync(CancellationToken cancellationToken)
    {
        var folder = Path.Combine(FileSystem.AppDataDirectory, "maps", "templates");
        Directory.CreateDirectory(folder);

        var templatePath = Path.Combine(folder, "default-offline-map.html");
        if (!File.Exists(templatePath))
        {
            await File.WriteAllTextAsync(templatePath, BuildFallbackTemplate(), cancellationToken);
        }

        return templatePath;
    }

    private static string Sanitize(string value)
    {
        var invalid = Path.GetInvalidFileNameChars();
        return string.Concat(value.Select(character => invalid.Contains(character) ? '-' : character));
    }

    private static string BuildFallbackTemplate() =>
        """
<!doctype html>
<html lang="vi">
<head>
  <meta charset="utf-8">
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <title>Quan 4 Offline Map</title>
  <style>
    :root { color-scheme: light; }
    * { box-sizing: border-box; }
    body {
      margin: 0;
      font-family: "Segoe UI", sans-serif;
      background: linear-gradient(180deg, #fdf4e7 0%, #fffdf8 100%);
      color: #1f2937;
    }
    .layout {
      display: grid;
      grid-template-rows: auto 1fr auto;
      min-height: 100vh;
    }
    header {
      padding: 14px 16px 10px;
      border-bottom: 1px solid rgba(251, 146, 60, 0.28);
      background: rgba(255, 255, 255, 0.92);
    }
    header h1 {
      margin: 0;
      font-size: 18px;
      font-weight: 700;
    }
    header p {
      margin: 6px 0 0;
      color: #6b7280;
      font-size: 12px;
    }
    .map-shell {
      position: relative;
      margin: 12px;
      border-radius: 20px;
      overflow: hidden;
      border: 1px solid rgba(148, 163, 184, 0.28);
      background:
        radial-gradient(circle at top left, rgba(251, 146, 60, 0.16), transparent 34%),
        radial-gradient(circle at bottom right, rgba(45, 212, 191, 0.18), transparent 32%),
        linear-gradient(135deg, #eff6ff 0%, #fff7ed 100%);
      box-shadow: 0 22px 48px rgba(15, 23, 42, 0.12);
    }
    .map-shell::before {
      content: "";
      position: absolute;
      inset: 0;
      background-image:
        linear-gradient(rgba(148, 163, 184, 0.10) 1px, transparent 1px),
        linear-gradient(90deg, rgba(148, 163, 184, 0.10) 1px, transparent 1px);
      background-size: 36px 36px;
      pointer-events: none;
    }
    svg {
      display: block;
      width: 100%;
      height: min(66vh, 420px);
    }
    .river {
      fill: none;
      stroke: rgba(59, 130, 246, 0.32);
      stroke-width: 20;
      stroke-linecap: round;
    }
    .district {
      fill: rgba(255, 255, 255, 0.56);
      stroke: rgba(251, 146, 60, 0.42);
      stroke-width: 3;
    }
    .poi {
      cursor: pointer;
      transition: transform 140ms ease;
    }
    .poi circle {
      fill: #f97316;
      stroke: #fff;
      stroke-width: 3;
      filter: drop-shadow(0 6px 10px rgba(249, 115, 22, 0.3));
    }
    .poi.active circle { fill: #0f766e; }
    .user circle {
      fill: #2563eb;
      stroke: #fff;
      stroke-width: 3;
    }
    .user .pulse {
      fill: rgba(37, 99, 235, 0.18);
      stroke: none;
    }
    .tag {
      position: absolute;
      padding: 6px 10px;
      border-radius: 999px;
      background: rgba(255, 255, 255, 0.92);
      border: 1px solid rgba(251, 146, 60, 0.24);
      box-shadow: 0 12px 24px rgba(15, 23, 42, 0.08);
      font-size: 12px;
      font-weight: 600;
    }
    .tag.q4 { left: 16px; bottom: 16px; }
    .tag.user { right: 16px; top: 16px; color: #2563eb; }
    .detail {
      padding: 14px 16px 18px;
      background: rgba(255, 255, 255, 0.96);
      border-top: 1px solid rgba(148, 163, 184, 0.18);
    }
    .detail h2 {
      margin: 0;
      font-size: 16px;
    }
    .detail p {
      margin: 6px 0 0;
      font-size: 13px;
      line-height: 1.5;
      color: #4b5563;
    }
    .muted {
      color: #6b7280;
    }
  </style>
</head>
<body>
  <div class="layout">
    <header>
      <h1>Ban do offline Quan 4</h1>
      <p>Pack: __PACK_NAME__</p>
    </header>
    <section class="map-shell">
      <div class="tag q4">Quan 4 offline</div>
      <div class="tag user" id="userTag" hidden>Vi tri cua ban</div>
      <svg id="map" viewBox="0 0 1000 700" preserveAspectRatio="xMidYMid meet" aria-label="Offline map">
        <path class="river" d="M80 140 C 260 60, 460 80, 640 190 C 770 270, 860 360, 930 520" />
        <path class="district" d="M160 140 C 330 120, 470 130, 620 220 C 730 285, 790 360, 820 470 C 742 564, 612 620, 466 616 C 312 610, 198 548, 132 444 C 110 326, 118 226, 160 140 Z" />
      </svg>
    </section>
    <section class="detail" id="detail">
      <h2>Chua chon diem POI</h2>
      <p class="muted">Chon marker trong ban do de xem thong tin tom tat.</p>
    </section>
  </div>
  <script>
    const pois = __POI_DATA__;
    const userLocation = __USER_LOCATION__;
    const svg = document.getElementById("map");
    const detail = document.getElementById("detail");
    const userTag = document.getElementById("userTag");
    const width = 1000;
    const height = 700;
    const padding = 96;

    function getBounds(items) {
      const points = items.filter(item => Number.isFinite(item.latitude) && Number.isFinite(item.longitude));
      if (!points.length) {
        return { minLat: 10.75, maxLat: 10.765, minLng: 106.698, maxLng: 106.71 };
      }

      return {
        minLat: Math.min(...points.map(item => item.latitude)),
        maxLat: Math.max(...points.map(item => item.latitude)),
        minLng: Math.min(...points.map(item => item.longitude)),
        maxLng: Math.max(...points.map(item => item.longitude))
      };
    }

    function project(latitude, longitude, bounds) {
      const lngRange = Math.max(bounds.maxLng - bounds.minLng, 0.001);
      const latRange = Math.max(bounds.maxLat - bounds.minLat, 0.001);
      const x = padding + ((longitude - bounds.minLng) / lngRange) * (width - padding * 2);
      const y = height - padding - ((latitude - bounds.minLat) / latRange) * (height - padding * 2);
      return { x, y };
    }

    function setActive(poi) {
      document.querySelectorAll(".poi").forEach(node => node.classList.toggle("active", node.dataset.id === poi.id));
      detail.innerHTML =
        "<h2>" + poi.name + "</h2>" +
        "<p>" + (poi.description || "Khong co mo ta.") + "</p>" +
        "<p><strong>Dia chi:</strong> " + poi.displayAddress + "</p>";
    }

    const normalizedPois = Array.isArray(pois) ? pois : [];
    const userPayload = userLocation && Number.isFinite(userLocation.latitude) && Number.isFinite(userLocation.longitude)
      ? [{ latitude: userLocation.latitude, longitude: userLocation.longitude }]
      : [];
    const bounds = getBounds(normalizedPois.concat(userPayload));

    normalizedPois.forEach(poi => {
      const point = project(poi.latitude, poi.longitude, bounds);
      const group = document.createElementNS("http://www.w3.org/2000/svg", "g");
      group.setAttribute("class", "poi");
      group.dataset.id = poi.id;

      const circle = document.createElementNS("http://www.w3.org/2000/svg", "circle");
      circle.setAttribute("cx", point.x);
      circle.setAttribute("cy", point.y);
      circle.setAttribute("r", "14");
      group.appendChild(circle);

      const label = document.createElementNS("http://www.w3.org/2000/svg", "text");
      label.setAttribute("x", point.x + 18);
      label.setAttribute("y", point.y + 4);
      label.setAttribute("font-size", "14");
      label.setAttribute("font-weight", "700");
      label.setAttribute("fill", "#111827");
      label.textContent = poi.name;
      group.appendChild(label);

      group.addEventListener("click", () => setActive(poi));
      svg.appendChild(group);
    });

    if (userPayload.length) {
      const point = project(userLocation.latitude, userLocation.longitude, bounds);
      const pulse = document.createElementNS("http://www.w3.org/2000/svg", "circle");
      pulse.setAttribute("class", "pulse");
      pulse.setAttribute("cx", point.x);
      pulse.setAttribute("cy", point.y);
      pulse.setAttribute("r", "28");

      const marker = document.createElementNS("http://www.w3.org/2000/svg", "g");
      marker.setAttribute("class", "user");

      const dot = document.createElementNS("http://www.w3.org/2000/svg", "circle");
      dot.setAttribute("cx", point.x);
      dot.setAttribute("cy", point.y);
      dot.setAttribute("r", "12");

      marker.appendChild(pulse);
      marker.appendChild(dot);
      svg.appendChild(marker);
      userTag.hidden = false;
    }

    if (normalizedPois.length) {
      setActive(normalizedPois[0]);
    }
  </script>
</body>
</html>
""";
}
