import { Pause, Play, Volume2 } from 'lucide-react';
import { useEffect, useRef, useState } from 'react';
import { normalizeMediaUrl } from '../../utils/media';

type AudioPlayerProps = {
  audioUrl?: string;
  text?: string;
  lang: string;
  onPlay: (mode: 'audio' | 'tts') => void;
  autoplay?: boolean;
  loading?: boolean;
  errorText?: string;
};

const narrationLangMap: Record<string, string> = {
  vi: 'vi-VN',
  en: 'en-US',
  zh: 'zh-CN',
  ja: 'ja-JP',
  ko: 'ko-KR',
};

function resolveVoice(lang: string, voices: SpeechSynthesisVoice[]) {
  const preferredLang = (narrationLangMap[lang] ?? lang).toLowerCase();
  const baseLang = preferredLang.split('-')[0];

  return (
    voices.find((voice) => voice.lang.toLowerCase() === preferredLang) ??
    voices.find((voice) => voice.lang.toLowerCase().startsWith(`${baseLang}-`)) ??
    voices.find((voice) => voice.lang.toLowerCase() === baseLang) ??
    null
  );
}

export function AudioPlayer({ audioUrl, text, lang, onPlay, autoplay = false, loading = false, errorText }: AudioPlayerProps) {
  const audio = useRef<HTMLAudioElement>(null);
  const autoStarted = useRef(false);
  const utterance = useRef<SpeechSynthesisUtterance | null>(null);
  const [playing, setPlaying] = useState(false);
  const [ttsVoice, setTtsVoice] = useState<SpeechSynthesisVoice | null>(null);
  const [voiceChecked, setVoiceChecked] = useState(false);
  const [playbackError, setPlaybackError] = useState<string>('');
  const narrationText = text?.trim() || '';
  const hasSpeechSynthesis = typeof window !== 'undefined' && 'speechSynthesis' in window;
  const hasNarration = Boolean(narrationText);
  const hasAudio = Boolean(audioUrl);
  const hasVietnameseVoice = Boolean(ttsVoice);
  const canUseTts = hasNarration && hasSpeechSynthesis && hasVietnameseVoice;

  useEffect(() => {
    if (!hasSpeechSynthesis) {
      setTtsVoice(null);
      setVoiceChecked(true);
      return;
    }

    const syncVoice = () => {
      const nextVoice = resolveVoice(lang, window.speechSynthesis.getVoices());
      setTtsVoice(nextVoice);
      setVoiceChecked(true);
    };

    syncVoice();
    window.speechSynthesis.addEventListener('voiceschanged', syncVoice);

    return () => {
      window.speechSynthesis.removeEventListener('voiceschanged', syncVoice);
    };
  }, [hasSpeechSynthesis, lang]);

  useEffect(() => {
    autoStarted.current = false;
    if (hasSpeechSynthesis) {
      window.speechSynthesis.cancel();
    }

    if (audio.current) {
      audio.current.pause();
      audio.current.currentTime = 0;
    }

    setPlaying(false);
    setPlaybackError('');
  }, [audioUrl, narrationText, hasSpeechSynthesis, lang]);

  useEffect(() => {
    return () => {
      if (hasSpeechSynthesis) {
        window.speechSynthesis.cancel();
      }
    };
  }, [hasSpeechSynthesis]);

  useEffect(() => {
    if (!autoplay || autoStarted.current) {
      return;
    }

    if (hasAudio) {
      autoStarted.current = true;
      void startNarration();
      return;
    }

    if (loading) {
      return;
    }

    if (!voiceChecked && hasNarration && hasSpeechSynthesis) {
      return;
    }

    if (!canUseTts) {
      return;
    }

    autoStarted.current = true;
    void startNarration();
  }, [autoplay, canUseTts, hasAudio, hasNarration, hasSpeechSynthesis, voiceChecked]);

  if (!hasAudio && !hasNarration) {
    return (
      <div className="rounded-2xl bg-slate-100 p-4 text-sm text-slate-500 dark:bg-slate-800">
        Dia diem nay chua co noi dung thuyet minh.
      </div>
    );
  }

  if (!hasAudio && !hasSpeechSynthesis) {
    return (
      <div className="rounded-2xl bg-slate-100 p-4 text-sm text-slate-500 dark:bg-slate-800">
        Trinh duyet nay khong ho tro doc thuyet minh.
      </div>
    );
  }

  if (!hasAudio && loading) {
    return (
      <div className="rounded-2xl bg-slate-100 p-4 text-sm text-slate-500 dark:bg-slate-800">
        Dang tai audio thuyet minh tieng Viet tu backend...
      </div>
    );
  }

  if (!hasAudio && voiceChecked && !hasVietnameseVoice) {
    return (
      <div className="rounded-2xl bg-slate-100 p-4 text-sm text-slate-500 dark:bg-slate-800">
        Chua lay duoc audio tu backend, va may nay cung chua co giong doc tieng Viet.
      </div>
    );
  }

  const stopNarration = () => {
    if (audio.current) {
      audio.current.pause();
      audio.current.currentTime = 0;
    }

    if (hasSpeechSynthesis) {
      window.speechSynthesis.cancel();
    }

    setPlaying(false);
  };

  const startAudio = async () => {
    if (!audio.current || !hasAudio) {
      return;
    }

    setPlaybackError('');

    try {
      await audio.current.play();
      setPlaying(true);
      onPlay('audio');
    } catch {
      setPlaying(false);
      setPlaybackError('Khong phat duoc file audio nay. Hay thu tai lai trang.');
    }
  };

  const startTts = async () => {
    if (!canUseTts || !hasSpeechSynthesis) {
      return;
    }

    window.speechSynthesis.cancel();
    setPlaybackError('');

    const nextUtterance = new SpeechSynthesisUtterance(narrationText);
    nextUtterance.lang = narrationLangMap[lang] ?? lang;
    if (ttsVoice) {
      nextUtterance.voice = ttsVoice;
    }
    nextUtterance.onstart = () => {
      setPlaying(true);
      onPlay('tts');
    };
    nextUtterance.onend = () => {
      setPlaying(false);
    };
    nextUtterance.onerror = () => {
      setPlaying(false);
      setPlaybackError('Trinh duyet khong doc duoc noi dung thuyet minh luc nay.');
    };

    utterance.current = nextUtterance;
    window.speechSynthesis.speak(nextUtterance);
  };

  async function startNarration() {
    if (hasAudio) {
      await startAudio();
      return;
    }

    await startTts();
  }

  const toggle = async () => {
    if (playing) {
      stopNarration();
      return;
    }

    await startNarration();
  };

  return (
    <div className="flex items-center gap-4 rounded-2xl bg-teal/10 p-4">
      <button
        onClick={toggle}
        className="grid h-11 w-11 place-items-center rounded-full bg-teal text-white"
      >
        {playing ? <Pause size={19} /> : <Play size={19} />}
      </button>

      <div className="min-w-0 flex-1">
        <p className="flex items-center gap-2 text-sm font-bold">
          <Volume2 size={16} />
          {hasAudio ? 'Audio thuyet minh' : 'Thuyet minh bang giong doc'}
        </p>
        <p className="text-xs text-slate-500">
          {hasAudio
            ? 'Phat audio tieng Viet do backend tao tu noi dung thuyet minh.'
            : voiceChecked
              ? 'Doc tu kich ban thuyet minh bang giong tieng Viet cua trinh duyet.'
              : 'Dang kiem tra giong doc tieng Viet tren trinh duyet.'}
        </p>
        {errorText || playbackError ? (
          <p className="mt-1 text-xs text-rose-600">{errorText || playbackError}</p>
        ) : null}
      </div>

      <audio
        ref={audio}
        src={normalizeMediaUrl(audioUrl)}
        preload="metadata"
        onPause={() => setPlaying(false)}
        onEnded={() => {
          setPlaying(false);
        }}
        onError={() => {
          setPlaying(false);
          setPlaybackError('File audio dang loi hoac khong con truy cap duoc.');
        }}
      />
    </div>
  );
}
