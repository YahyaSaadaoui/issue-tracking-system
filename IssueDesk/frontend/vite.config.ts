import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";
import tailwind from "@tailwindcss/vite";
import { fileURLToPath, URL } from "node:url";

const proxyTarget = process.env.VITE_PROXY_TARGET ?? "http://localhost:5080";

export default defineConfig({
  plugins: [react(), tailwind()],
  resolve: {
    alias: {
      "@": fileURLToPath(new URL("./src", import.meta.url)),
    },
  },
  server: {
    host: true,
    port: 5173,
    proxy: { "/api": { target: proxyTarget, changeOrigin: true, secure: false } },
  },
});
