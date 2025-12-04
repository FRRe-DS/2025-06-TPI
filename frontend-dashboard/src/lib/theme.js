import { browser } from '$app/environment';

export function initTheme() {
  if (!browser) return;
  const saved = localStorage.getItem('theme');
  if (saved === 'light') {
    document.documentElement.setAttribute('data-theme', 'light');
  } else {
    document.documentElement.removeAttribute('data-theme');
  }
}

export function toggleTheme() {
  if (!browser) return;
  const current = localStorage.getItem('theme');
  if (current === 'light') {
    localStorage.removeItem('theme');
    document.documentElement.removeAttribute('data-theme');
  } else {
    localStorage.setItem('theme', 'light');
    document.documentElement.setAttribute('data-theme', 'light');
  }
}

export function isLight() {
  if (!browser) return false; // Default to dark theme on server
  return document.documentElement.getAttribute('data-theme') === 'light' || localStorage.getItem('theme') === 'light';
}
