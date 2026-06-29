import { useEffect, useRef, useState } from 'react';
import { QrCode } from 'lucide-react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { qrApi } from '../api/qrApi';
import { TextInput } from '../components/common/FormControls';
import { StatusPill } from '../components/common/StatusPill';
import { getCopy } from '../i18n/copy';
import { useAppStore } from '../store/appStore';
import type { QrActivationResponse } from '../types/responses';
import { track } from '../utils/analytics';

export default function QrPage() {
  const { lang } = useAppStore();
  const navigate = useNavigate();
  const ui = getCopy(lang);
  const [searchParams] = useSearchParams();
  const [code, setCode] = useState('');
  const [statusText, setStatusText] = useState<string>(ui.qr.startingCamera);
  const [lastResolved, setLastResolved] = useState<QrActivationResponse | null>(null);
  const [errorText, setErrorText] = useState('');
  const scanLockRef = useRef(false);
  const scannerStartedRef = useRef(false);
  const scannerRef = useRef<{
    stop: () => Promise<void>;
    clear: () => void;
  } | null>(null);
  const resolveRef = useRef<(value: string) => Promise<void>>(async () => undefined);
  const codeFromQuery = searchParams.get('code')?.trim() ?? '';

  const resolveValue = async (rawValue: string) => {
    const value = rawValue.trim();
    if (!value || scanLockRef.current) {
      return;
    }

    scanLockRef.current = true;
    setErrorText('');

    try {
      const qrCodeFromUrl = (() => {
        try {
          const url = new URL(value);
          if (!/^https?:$/i.test(url.protocol)) {
            return null;
          }

          if (/\/qr\/?$/i.test(url.pathname)) {
            return url.searchParams.get('code');
          }

          const hash = url.hash.startsWith('#') ? url.hash.slice(1) : url.hash;
          if (hash) {
            const [hashPath, hashQuery = ''] = hash.split('?');
            if (/^\/qr\/?$/i.test(hashPath)) {
              return new URLSearchParams(hashQuery).get('code');
            }
          }

          return null;
        } catch {
          return null;
        }
      })();

      const poiIdFromCustomScheme = value.startsWith('quan4tourism://poi/')
        ? value.slice('quan4tourism://poi/'.length)
        : null;

      if (poiIdFromCustomScheme) {
        const poiId = poiIdFromCustomScheme;
        await track('qr_scanned', lang, poiId, { fallback: 'poi_id' });
        navigate(`/poi/${poiId}?autoplay=prefer_audio&source=qr`);
        return;
      }

      const normalizedCode = qrCodeFromUrl
        ?? (value.startsWith('quan4tourism://qr/')
          ? value.slice('quan4tourism://qr/'.length)
          : value);

      if (/^[a-f0-9]{24}$/i.test(normalizedCode)) {
        await track('qr_scanned', lang, normalizedCode, { fallback: 'poi_id' });
        navigate(`/poi/${normalizedCode}?autoplay=prefer_audio&source=qr`);
        return;
      }

      const activation = await qrApi.resolve(normalizedCode);
      setLastResolved(activation);
      await track('qr_scanned', lang, activation.poiId, {
        qrCode: activation.code,
        scanMode: activation.scanMode,
        activationTitle: activation.title,
      });

      navigate(`/poi/${activation.poiId}?autoplay=${encodeURIComponent(activation.scanMode)}&source=qr`);
    } catch (error) {
      setErrorText((error as Error).message);
      setStatusText(ui.qr.resolveFailed);
    } finally {
      scanLockRef.current = false;
    }
  };

  resolveRef.current = resolveValue;

  useEffect(() => {
    if (!codeFromQuery) {
      return;
    }

    setCode(codeFromQuery);
    setStatusText(ui.qr.openingFromLink);
    void resolveValue(codeFromQuery);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [codeFromQuery, ui.qr.openingFromLink]);

  useEffect(() => {
    let disposed = false;

    async function startScanner() {
      if (codeFromQuery) {
        return;
      }

      if (!navigator.mediaDevices?.getUserMedia) {
        setStatusText(ui.qr.cameraUnsupported);
        return;
      }

      try {
        const { Html5Qrcode } = await import('html5-qrcode');
        if (disposed) {
          return;
        }

        const scanner = new Html5Qrcode('qr-reader');
        scannerRef.current = scanner;
        setStatusText(ui.qr.cameraReady);

        await scanner.start(
          { facingMode: 'environment' },
          { fps: 10, qrbox: { width: 220, height: 220 } },
          (decodedText) => {
            void resolveRef.current(decodedText);
          },
          () => undefined,
        );
        scannerStartedRef.current = true;
      } catch {
        setStatusText(ui.qr.cameraFailed);
      }
    }

    void startScanner();

    return () => {
      disposed = true;
      const scanner = scannerRef.current;
      scannerRef.current = null;
      if (scanner) {
        if (scannerStartedRef.current) {
          scannerStartedRef.current = false;
          void scanner
            .stop()
            .catch(() => undefined)
            .finally(() => {
              scanner.clear();
            });
        } else {
          scanner.clear();
        }
      }
    };
  }, [codeFromQuery, ui.qr.cameraFailed, ui.qr.cameraReady, ui.qr.cameraUnsupported]);

  return (
    <section className="shell py-12">
      <p className="section-kicker">{ui.qr.kicker}</p>
      <h1 className="mt-2 text-4xl font-bold">{ui.qr.title}</h1>
      <p className="mt-3 max-w-2xl text-slate-500">
        {ui.qr.subtitle}
      </p>

      <div className="mt-8 grid gap-6 lg:grid-cols-[1.1fr_.9fr]">
        <div className="rounded-[2rem] bg-white p-6 shadow-soft dark:bg-slate-900">
          <div className="flex items-center gap-3">
            <div className="grid h-12 w-12 place-items-center rounded-2xl bg-coral/10 text-coral">
              <QrCode />
            </div>
            <div>
              <p className="text-sm font-bold uppercase tracking-[0.2em] text-slate-500">{ui.qr.cameraTitle}</p>
              <p className="text-sm text-slate-500">{statusText}</p>
            </div>
          </div>
          <div id="qr-reader" className="mt-5 overflow-hidden rounded-3xl border border-slate-200 dark:border-slate-700" />
        </div>

        <div className="rounded-[2rem] bg-white p-6 shadow-soft dark:bg-slate-900">
          <h2 className="text-2xl font-bold">{ui.qr.manualTitle}</h2>
          <p className="mt-2 text-sm text-slate-500">
            {ui.qr.manualSupport}
          </p>

          <form
            className="mt-5 grid gap-4"
            onSubmit={(event) => {
              event.preventDefault();
              void resolveValue(code);
            }}
          >
            <TextInput
              value={code}
              onChange={(event) => setCode(event.target.value)}
              placeholder={ui.qr.manualPlaceholder}
            />
            <button className="btn-primary justify-center">
              <QrCode size={16} />
              {ui.qr.openFromQr}
            </button>
          </form>

          {errorText ? <div className="mt-4 rounded-2xl bg-rose-50 px-4 py-3 text-sm text-rose-700">{errorText}</div> : null}

          {lastResolved ? (
            <div className="mt-5 rounded-3xl border border-slate-200 p-4 dark:border-slate-700">
              <div className="flex items-center justify-between gap-3">
                <div>
                  <p className="text-xs font-bold uppercase tracking-[0.2em] text-coral">{lastResolved.stopZone}</p>
                  <h3 className="mt-1 text-lg font-bold">{lastResolved.poiName}</h3>
                </div>
                <StatusPill status={lastResolved.isActive ? 'approved' : 'pending'} />
              </div>
              <p className="mt-2 text-sm text-slate-500">{lastResolved.title}</p>
              <p className="mt-1 text-sm text-slate-500">{lastResolved.poiAddress}</p>
            </div>
          ) : null}
        </div>
      </div>
    </section>
  );
}
