<script>
  import { onMount } from 'svelte';

  onMount(() => {
    const header = document.querySelector('header');
    const sidebar = document.querySelector('.sidebar');
    if (!header || !sidebar) return;

    const handleScroll = () => {
      const scrolledPastHeader = window.scrollY > header.offsetHeight;
      sidebar.classList.toggle('full', scrolledPastHeader);
    };

    window.addEventListener('scroll', handleScroll, { passive: true });
    // run once to set initial state
    handleScroll();

    return () => window.removeEventListener('scroll', handleScroll);
  });
</script>

<div class="layout">
  <aside class="sidebar">
    <details class="sidebar-details" open>
      <summary>Dashboard</summary>
      <nav>
        <ul>
          <li><a href="/dashboard/shipments">Pedidos</a></li>
          <li><a href="/dashboard/shipments/create">Crear Pedido</a></li>
          <li><a href="/transport-methods">Transport Methods</a></li>
        </ul>
      </nav>
    </details>
  </aside>
  <main class="content">
    <slot></slot>
  </main>
</div>

<style>
  .layout {
    display: block; /* content flow independent because sidebar is fixed */
  }
  .sidebar {
    width: 200px;
    background-color: #1e1e1e;
    padding: 1rem;
    border-right: 1px solid #444;
    position: fixed;
    left: 0;
    top: var(--header-height, 64px);
    height: calc(100vh - var(--header-height, 64px));
    overflow-y: auto;
    z-index: 20;
  }

  /* Cuando el usuario ha scrolleado más allá del header, la sidebar ocupa toda la ventana */
  .sidebar.full {
    top: 0;
    height: 100vh;
  }
  h2 {
    color: #f0f0f0;
    margin-bottom: 2rem;
  }
  nav ul {
    list-style: none;
    padding: 0;
  }
  nav a {
    display: block;
    padding: 0.75rem 1rem;
    color: #ccc;
    text-decoration: none;
    border-radius: 4px;
    margin-bottom: 0.5rem;
  }
  nav a:hover {
    background-color: #333;
    color: #fff;
  }
  .sidebar-details summary {
    cursor: pointer;
    padding: 0.5rem 0;
    color: #f0f0f0;
    font-weight: 600;
    list-style: none;
  }

  .sidebar-details summary::-webkit-details-marker {
    display: none;
  }

  .sidebar-details summary:after {
    content: '▾';
    float: right;
    transform: rotate(0deg);
    transition: transform 0.15s ease;
  }

  .sidebar-details[open] summary:after {
    transform: rotate(180deg);
  }
  .content {
    margin-left: 200px; /* espacio para la sidebar fija */
    padding: 2rem;
    overflow-y: auto;
    min-height: calc(100vh - var(--header-height, 64px));
  }
</style>