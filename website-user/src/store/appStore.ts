import { create } from 'zustand'
import type { Lang } from '../types/responses'
import { track } from '../utils/analytics'

type State = {
  theme: 'light' | 'dark'
  lang: Lang
  location?: { lat: number; lng: number }
  toggleTheme: () => void
  setLang: (lang: Lang) => void
  setLocation: (location: { lat: number; lng: number }) => void
}

const initialTheme = (localStorage.getItem('q4-theme') as 'light' | 'dark') || 'light'
document.documentElement.classList.toggle('dark', initialTheme === 'dark')

export const useAppStore = create<State>((set, get) => ({
  theme: initialTheme,
  lang: (localStorage.getItem('q4-lang') as Lang) || 'vi',
  toggleTheme: () => {
    const theme = get().theme === 'dark' ? 'light' : 'dark'
    localStorage.setItem('q4-theme', theme)
    document.documentElement.classList.toggle('dark', theme === 'dark')
    set({ theme })
  },
  setLang: (lang) => {
    const previousLanguage = get().lang
    if (lang === previousLanguage) return
    localStorage.setItem('q4-lang', lang)
    set({ lang })
    track('language_changed', lang, undefined, { previousLanguage })
  },
  setLocation: (location) => set({ location })
}))
