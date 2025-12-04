<script lang="ts">
  import { onMount, onDestroy, tick } from 'svelte';
  import { getShipmentStatusDistribution } from '$lib/services/shipmentService';
  import type { ShipmentStatusDistributionDto } from '$lib/types';

  // --- State Management ---
  let statusDistribution: ShipmentStatusDistributionDto[] = [];
  let error: string | null = null;
  let limit: number | null = null;
  let isLoading = true;

  // --- Charting ---
  let chartCanvas: HTMLCanvasElement;
  let chart: any; 
  let ChartConstructor: any;

  const filterOptions = [20, 50, 100, 1000];
  const statusTranslations: { [key: string]: string } = {
    in_transit: 'En Tránsito',
    created: 'Creado',
    delivered: 'Entregado',
    cancelled: 'Cancelado',
    in_distribution: 'En Distribución',
    arrived: 'Arribado',
    reserved: 'Reservado',
  };

  // --- Data Fetching ---
  async function fetchData(newLimit: number | null) {
    console.log('Fetching data with limit:', newLimit);
    
    // We do NOT destroy the chart here. We want to keep it alive
    // and just update its data. This prevents DOM thrashing.
    
    isLoading = true;
    limit = newLimit;

    try {
      const data = await getShipmentStatusDistribution(fetch, limit);
      console.log('Data received:', data);
      statusDistribution = data;
      error = null;
    } catch (e: any) {
      console.error('Error fetching data:', e);
      error = e.message || 'Ocurrió un error al cargar los datos.';
      // We keep the old data on error or clear it? 
      // Let's clear it to avoid confusion
      statusDistribution = [];
    } finally {
      isLoading = false;
    }
  }

  // --- Reactive Chart Updater ---
  $: if (chart && statusDistribution) {
    console.log('Updating chart with data', statusDistribution.length);
    chart.data.labels = statusDistribution.map(d => statusTranslations[d.status] || d.status);
    chart.data.datasets[0].data = statusDistribution.map(d => d.count);
    chart.update();
  }

  // --- Lifecycle & Initialization ---
  onMount(async () => {
    console.log('Component mounted');
    
    // 1. Dynamic Import of Chart.js (Client-side only)
    if (typeof window !== 'undefined') {
        try {
            const module = await import('chart.js');
            ChartConstructor = module.Chart;
            const { BarController, BarElement, CategoryScale, LinearScale, Tooltip, Legend, Title } = module;
            ChartConstructor.register(BarController, BarElement, CategoryScale, LinearScale, Tooltip, Legend, Title);
            console.log('Chart.js loaded and registered');
        } catch (err) {
            console.error('Failed to load Chart.js', err);
        }
    }

    // 2. Fetch initial data
    await fetchData(null);
  });

  // --- Chart Creation ---
  // This runs when:
  // 1. Canvas exists (it's bound)
  // 2. Chart.js is loaded
  // 3. Chart instance doesn't exist yet
  $: if (chartCanvas && ChartConstructor && !chart) {
    console.log('Initializing chart instance');
    const ctx = chartCanvas.getContext('2d');
    if (ctx) {
      chart = new ChartConstructor(ctx, {
        type: 'bar',
        data: {
          labels: [], 
          datasets: [{
            label: 'Cantidad de Pedidos',
            data: [], 
            backgroundColor: '#f07c13',
            borderColor: 'hsl(30, 88%, 41%)',
            borderWidth: 2,
          }]
        },
        options: {
          responsive: true,
          maintainAspectRatio: false,
          scales: { y: { beginAtZero: true } },
          animation: { duration: 500 },
          plugins: {
            legend: { display: false },
            title: { display: true, text: 'Distribución de Estados' }
          }
        }
      });
    }
  }

  onDestroy(() => {
    if (chart) chart.destroy();
  });
</script>

<div class="statistics-page">
  <div class="header-container">
    <h1>Estadísticas de Pedidos</h1>
    <div class="filter-controls">
      <button class:active={limit === null} on:click={() => fetchData(null)}>
        Todos
      </button>
      {#each filterOptions as option}
        <button class:active={limit === option} on:click={() => fetchData(option)}>
          Últimos {option}
        </button>
      {/each}
    </div>
  </div>

  <!-- Loading Indicator (Overlay) -->
  {#if isLoading}
    <div class="loading-indicator">
        <p>Cargando datos...</p>
    </div>
  {/if}

  {#if error}
    <div class="error-message">
      <p>{error}</p>
    </div>
  {/if}
  
  <!-- Main Content -->
  <!-- We keep this in the DOM even when loading to preserve the Chart instance -->
  <!-- We hide it only if there is NO data and NOT loading (e.g. empty result or error) -->
  <div class="content-wrapper" class:opacity-50={isLoading}>
      
      {#if statusDistribution.length > 0}
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
      {:else if !isLoading && !error}
        <p>No se encontraron datos de distribución.</p>
      {/if}
  </div>
</div>

<style>
  .statistics-page {
    animation: fadeIn 0.5s ease-out;
    position: relative;
  }

  .content-wrapper {
      transition: opacity 0.2s;
  }
  
  .opacity-50 {
      opacity: 0.5;
      pointer-events: none;
  }

  .loading-indicator {
      position: absolute;
      top: 100px;
      left: 50%;
      transform: translateX(-50%);
      background: rgba(255,255,255,0.9);
      padding: 1rem 2rem;
      border-radius: 8px;
      box-shadow: 0 4px 12px rgba(0,0,0,0.1);
      z-index: 10;
      font-weight: bold;
      color: var(--accent);
      border: 1px solid var(--border);
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
    margin-bottom: 0;
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
    margin-bottom: 1rem;
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
