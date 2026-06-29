import { getCopy } from '../../i18n/copy';
import { useAppStore } from '../../store/appStore';

export function ErrorBox({ text }: { text?: string }) {
  const lang = useAppStore((state) => state.lang);
  const ui = getCopy(lang);

  return (
    <div className="rounded-3xl border border-orange-200 bg-orange-50 p-8 text-center text-orange-800">
      {text || ui.common.loadError}
    </div>
  );
}
