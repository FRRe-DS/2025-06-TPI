<script lang="ts">
  import type { PageData } from './$types';
  import { onMount, onDestroy } from 'svelte';

  export let data: PageData;

  $: ({ statusDistribution, error } = data);

  let chartCanvas: HTMLCanvasElement;
  let chart: any;

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

  onMount(() => {
    if (chartCanvas && statusDistribution && typeof Chart !== 'undefined') {
      const ctx = chartCanvas.getContext('2d');
      if (ctx) {
        chart = new Chart(ctx, {
          type: 'bar',
          data: {
            // Traducir las etiquetas para el gráfico
            labels: statusDistribution.map(d => statusTranslations[d.status] || d.status),
            datasets: [{
              label: 'Cantidad de Pedidos',
              data: statusDistribution.map(d => d.count),
              backgroundColor: '#f07c13', // Usar el acento naranja del proyecto
              borderColor: 'hsl(30, 88%, 41%)', // Un tono de naranja más oscuro para el borde
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
</script>

<div class="statistics-page">
  <h1>Estadísticas de Pedidos</h1>

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
    <p>No se encontraron datos de distribución de estados.</p>
  {/if}

</div>

<style>
  .statistics-page {
    animation: fadeIn 0.5s ease-out;
  }

  h1 {
    color: var(--text);
    margin-bottom: 2rem;
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
    color: var(--primary);
    margin-bottom: 0.5rem;
  }

  .kpi-label {
    font-size: 1rem;
    color: var(--muted);
    /* Removido text-transform: capitalize para permitir traducción manual */
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
