<div class="layout">
  <aside class="sidebar">
    <h2>Dashboard</h2>
    <nav>
      <ul>
        <li><a href="/dashboard/shipments">Pedidos</a></li>
        <li><a href="/dashboard/shipments/create">Crear Pedido</a></li>
        <!-- Future options -->
        <li><a href="/transport-methods">Transport Methods</a></li>
      </ul>
    </nav>
  </aside>
  <main class="content">
    <slot></slot>
  </main>
</div>

<style>
  .layout {
    display: flex;
    height: calc(100vh - 4rem); /* Assuming a header of 4rem */
  }
  .sidebar {
    width: 200px;
    background-color: #1e1e1e;
    padding: 1rem;
    border-right: 1px solid #444;
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
  .content {
    flex: 1;
    padding: 2rem;
    overflow-y: auto;
  }
</style>