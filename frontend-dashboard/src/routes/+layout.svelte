
<script>
  import '../app.css';
  import { onMount, onDestroy } from 'svelte';
  import logoUrl from '$lib/assets/papa.png?url';
  import { initTheme } from '$lib/theme.js';

  onMount(() => {
    const header = document.querySelector('header');
    if (!header) return;

    // inicializar tema guardado (modo oscuro por defecto)
    initTheme();

    const setHeaderHeight = () => {
      const h = header.offsetHeight;
      document.documentElement.style.setProperty('--header-height', `${h}px`);
    };

    setHeaderHeight();
    window.addEventListener('resize', setHeaderHeight);

    onDestroy(() => window.removeEventListener('resize', setHeaderHeight));
  });
</script>

<header>
  <div class="header-inner">
    <div class="logo">
      <img src={logoUrl} alt="Logo Gestor de Papas" />
    </div>
    <h1>Gestor de Papas</h1>
  </div>
</header>

<main>
  <slot />
</main>

<style>
  header {
    background-color: var(--header-bg);
    padding: 1rem 2rem;
    position: fixed;
    top: 0;
    left: 0;
    right: 0;
    z-index: 30;
    border-bottom: 1px solid var(--border);
  }

  .header-inner {
    display: flex;
    align-items: center;
    gap: 1rem;
    justify-content: flex-start;
  }

  .logo {
    display: flex;
    align-items: center;
  }

  .logo img {
    height: calc(var(--header-height, 64px) * 0.5);
    width: auto;
    display: block;
  }

  .theme-toggle {
    background: transparent;
    color: var(--text);
    border: 1px solid var(--border);
    padding: 0.4rem 0.6rem;
    border-radius: 6px;
    cursor: pointer;
    font-weight: 600;
  }
  .theme-toggle:active { transform: translateY(1px); }

  main {
    padding: 2rem;
    padding-top: calc(var(--header-height, 64px) + 1rem);
  }
</style>