import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import { VitePWA } from 'vite-plugin-pwa'
export default defineConfig({
  server: { port: 5174, strictPort: true },
  plugins: [react(), VitePWA({ registerType: 'autoUpdate', manifest: { name: 'Quan4 Culinary Tourism', short_name: 'Quan4 Food', theme_color: '#FF6B35', background_color: '#0F172A', display: 'standalone', icons: [{ src: 'favicon.svg', sizes: 'any', type: 'image/svg+xml', purpose: 'any' }] } })]
})
