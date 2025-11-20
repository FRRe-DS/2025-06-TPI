
<script>
  import '../app.css';
  import { onMount, onDestroy } from 'svelte';
  import logoUrl from '$lib/assets/papa.png?url';

  onMount(() => {
    const header = document.querySelector('header');
    if (!header) return;

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
    background-color: #2a2a2a;
    padding: 1rem 2rem;
    position: fixed;
    top: 0;
    left: 0;
    right: 0;
    z-index: 30;
    border-bottom: 1px solid #444;
  }

  .header-inner {
    display: flex;
    align-items: center;
    gap: 1rem;
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

  main {
    padding: 2rem;
    padding-top: calc(var(--header-height, 64px) + 1rem);
  }
</style>