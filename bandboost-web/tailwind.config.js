/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        "on-secondary-container": "#514f81",
        "surface": "#fcf8ff",
        "error": "#ba1a1a",
        "error-container": "#ffdad6",
        "primary": "#3525cd",
        "background": "#fcf8ff",
        "on-surface-variant": "#464555",
        "on-surface": "#1b1b24",
        "outline-variant": "#c7c4d8",
        // ... (Bạn có thể copy toàn bộ các màu khác từ thẻ <script> ở file HTML cũ vào đây để đầy đủ nhất)
      },
      fontFamily: {
        "headline-xl": ["Inter"],
        "body-md": ["Inter"],
        "button-text": ["Inter"],
        "headline-md": ["Inter"],
        "body-lg": ["Inter"],
        "headline-lg": ["Inter"],
        "label-md": ["Inter"]
      }
    },
  },
  plugins: [],
}