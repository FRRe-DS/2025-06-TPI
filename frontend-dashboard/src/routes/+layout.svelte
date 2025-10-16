<script>
  import '../app.css';
</script>

<header>
  <h1>Gestor de Papas</h1>
</header>

<main>
  <slot />
</main>

<style>
  header {
    background-color: #f8f9fa;
    padding: 1rem 2rem;
    border-bottom: 1px solid #dee2e6;
  }

  main {
    padding: 2rem;
  }
</style>