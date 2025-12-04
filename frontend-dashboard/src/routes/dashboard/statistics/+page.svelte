<script lang="ts">
  import type { PageData } from './$types';
  import { onMount, onDestroy } from 'svelte';
  import { goto } from '$app/navigation';

  export let data: PageData;

  $: ({ statusDistribution, error, limit } = data);

  let chartCanvas: HTMLCanvasElement;
  let chart: any;

  const filterOptions = [20, 50, 100, 1000];

  // Mapa de traducciones de estados
  const statusTranslations: { [key: string]: string } = {
    in_transit: 'En Tránsito',
    created: 'Creado',
    delivered: 'Entregado',
    cancelled: 'Cancelado',
    in_distribution: 'En Distribución',
    arrived: 'Arribado',
    reserved: 'Reservado',
  };

  // --- Bloque Reactivo para Actualizar el Gráfico ---
  $: if (chart && statusDistribution) {
    chart.data.labels = statusDistribution.map(d => statusTranslations[d.status] || d.status);
    chart.data.datasets[0].data = statusDistribution.map(d => d.count);
    chart.update(); // Re-renderiza el gráfico con los nuevos datos
  }

  onMount(() => {
    // --- Creación Inicial del Gráfico ---
    if (chartCanvas && statusDistribution && typeof Chart !== 'undefined') {
      const ctx = chartCanvas.getContext('2d');
      if (ctx) {
        chart = new Chart(ctx, {
          type: 'bar',
          data: {
            labels: statusDistribution.map(d => statusTranslations[d.status] || d.status),
            datasets: [{
              label: 'Cantidad de Pedidos',
              data: statusDistribution.map(d => d.count),
              backgroundColor: '#f07c13',
              borderColor: 'hsl(30, 88%, 41%)',
              borderWidth: 2,
            }]
          },
          options: {
            responsive: true,
            maintainAspectRatio: false,
            scales: {
              y: {
                beginAtZero: true
              }
            }
          }
        });
      }
    }
  });

  onDestroy(() => {
    if (chart) {
      chart.destroy();
    }
  });

  function applyFilter(newLimit: number | null) {
    if (newLimit) {
      goto(`/dashboard/statistics?limit=${newLimit}`, { keepFocus: true, noScroll: true });
    } else {
      goto('/dashboard/statistics', { keepFocus: true, noScroll: true });
    }
  }
</script>

<div class="statistics-page">
  <div class="header-container">
    <h1>Estadísticas de Pedidos</h1>
    <div class="filter-controls">
      <button class:active={!limit} on:click={() => applyFilter(null)}>
        Todos
      </button>
      {#each filterOptions as option}
        <button class:active={limit === option} on:click={() => applyFilter(option)}>
          Últimos {option}
        </button>
      {/each}
    </div>
  </div>

  {#if error}
    <div class="error-message">
      <p>{error}</p>
    </div>
  {:else if statusDistribution && statusDistribution.length > 0}
    <div class="kpi-grid">
      {#each statusDistribution as item}
        <div class="kpi-card">
          <span class="kpi-value">{item.count}</span>
          <span class="kpi-label">
            {statusTranslations[item.status] || item.status}
          </span>
        </div>
      {/each}
    </div>

    <div class="chart-container">
      <canvas bind:this={chartCanvas}></canvas>
    </div>
  {:else}
    <p>No se encontraron datos de distribución para el filtro seleccionado.</p>
  {/if}

</div>

<style>
  .statistics-page {
    animation: fadeIn 0.5s ease-out;
  }

  .header-container {
    display: flex;
    justify-content: space-between;
    align-items: center;
    flex-wrap: wrap;
    gap: 1rem;
    margin-bottom: 2rem;
  }

  h1 {
    color: var(--text);
    margin-bottom: 0; /* Ajustado por el flex container */
  }

  .filter-controls {
    display: flex;
    gap: 0.5rem;
    background-color: var(--card);
    padding: 0.5rem;
    border-radius: 8px;
    border: 1px solid var(--border);
  }

  .filter-controls button {
    background-color: transparent;
    color: var(--muted);
    border: none;
    padding: 0.5rem 1rem;
    border-radius: 6px;
    cursor: pointer;
    font-weight: 600;
    transition: background-color 0.2s, color 0.2s;
  }

  .filter-controls button:hover {
    color: var(--text);
  }

  .filter-controls button.active {
    background-color: var(--accent);
    color: var(--card);
  }

  .error-message {
    background-color: var(--error-bg);
    color: var(--error-text);
    padding: 1rem;
    border-radius: 8px;
    border: 1px solid var(--error-border);
  }

  .kpi-grid {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(180px, 1fr));
    gap: 1.5rem;
    margin-bottom: 2rem;
  }

  .kpi-card {
    background-color: var(--card);
    border: 1px solid var(--border);
    border-radius: 8px;
    padding: 1.5rem;
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    text-align: center;
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
    transition: transform 0.2s, box-shadow 0.2s;
  }

  .kpi-card:hover {
    transform: translateY(-5px);
    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
  }

  .kpi-value {
    font-size: 2.5rem;
    font-weight: 700;
    color: var(--accent);
  }

  .kpi-label {
    font-size: 1rem;
    color: var(--muted);
  }

  .chart-container {
    background-color: var(--card);
    border: 1px solid var(--border);
    border-radius: 8px;
    padding: 2rem;
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
    height: 500px;
  }

  @keyframes fadeIn {
    from {
      opacity: 0;
      transform: translateY(-10px);
    }
    to {
      opacity: 1;
      transform: translateY(0);
    }
  }
</style>
