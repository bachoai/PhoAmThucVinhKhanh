import { useEffect, useRef } from 'react';
import { useLocation } from 'react-router-dom';
import { useAppStore } from '../store/appStore';
import { sendPresencePing } from '../utils/analytics';

export function PresenceHeartbeat() {
  const location = useLocation();
  const lang = useAppStore((state) => state.lang);
  const isAuthenticated = useAppStore((state) => state.isAuthenticated);
  const pageViewIdRef = useRef(crypto.randomUUID());
  const currentPath = `${location.pathname}${location.search}`;

  useEffect(() => {
    pageViewIdRef.current = crypto.randomUUID();
  }, [currentPath]);

  useEffect(() => {
    let intervalId: number | undefined;

    const send = () => {
      if (document.visibilityState !== 'visible') {
        return;
      }

      sendPresencePing(
        lang,
        {
          path: currentPath,
          title: document.title,
          isAuthenticated,
        },
        pageViewIdRef.current,
      );
    };

    const stop = () => {
      if (intervalId) {
        window.clearInterval(intervalId);
        intervalId = undefined;
      }
    };

    const start = () => {
      stop();
      send();
      intervalId = window.setInterval(send, 20000);
    };

    const handleVisibilityChange = () => {
      if (document.visibilityState === 'visible') {
        start();
        return;
      }

      stop();
    };

    const handleFocus = () => send();

    handleVisibilityChange();
    document.addEventListener('visibilitychange', handleVisibilityChange);
    window.addEventListener('focus', handleFocus);

    return () => {
      stop();
      document.removeEventListener('visibilitychange', handleVisibilityChange);
      window.removeEventListener('focus', handleFocus);
    };
  }, [currentPath, isAuthenticated, lang]);

  return null;
}
