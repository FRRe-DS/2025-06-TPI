<script lang="ts">
  import { page } from '$app/stores';
  import { get } from 'svelte/store';
  import { onMount } from 'svelte';
  import { getShipmentById } from '$lib/services/shipmentService';
  import type { Shipment } from '$lib/types';

  let shipmentDetails: Shipment | null = null;
  let error = '';

  onMount(async () => {
    const shipmentId = get(page).params.id;
    try {
      shipmentDetails = await getShipmentById(shipmentId);
      if (!shipmentDetails) {
        error = 'Envío no encontrado';
      }
    } catch (e) {
      error = 'Error al cargar el envío';
    }
  });
</script>

{#if error}
  <p style="color: red;">{error}</p>
{:else if !shipmentDetails}
  <p>Cargando...</p>
{:else}
  <div class="details-container">
    <h2>Detalles del Pedido {shipmentDetails.id}</h2>

    <div class="details-section">
      <h3>Información General</h3>
      <p><strong>Destino:</strong> {shipmentDetails.destination}</p>
      <p><strong>Estado Actual:</strong> {shipmentDetails.status}</p>
      <p><strong>Fecha de Ingreso:</strong> {shipmentDetails.entryDate}</p>
    </div>

    {#if shipmentDetails.estimatedDelivery}
      <div class="details-section">
        <h3>Fecha Estimada de Entrega</h3>
        <p>{shipmentDetails.estimatedDelivery}</p>
      </div>
    {/if}

    {#if shipmentDetails.history}
      <div class="details-section">
        <h3>Historial de Estados</h3>
        <ul>
          {#each shipmentDetails.history as event}
            <li><strong>{event.date}:</strong> {event.status} - {event.description}</li>
          {/each}
        </ul>
      </div>
    {/if}

    {#if shipmentDetails.products}
      <div class="details-section">
        <h3>Productos</h3>
        <ul>
          {#each shipmentDetails.products as product}
            <li>{product.name} (Cantidad: {product.quantity})</li>
          {/each}
        </ul>
      </div>
    {/if}

    <a href="/shipments" class="button">Volver al Listado</a>
  </div>
{/if}

<style>
  .details-container {
    padding: 2rem;
    background-color: #1e1e1e;
    border-radius: 8px;
    border: 1px solid #444;
    max-width: 700px;
    margin: auto;
  }

  .details-section {
    margin-bottom: 1.5rem;
  }

  h2, h3 {
    color: #f0f0f0;
  }

  p, li {
    color: #ccc;
  }

  ul {
    list-style-type: none;
    padding-left: 0;
  }

  .button {
    display: inline-block;
    background-color: #3b82f6;
    color: white;
    padding: 10px 15px;
    border-radius: 4px;
    text-decoration: none;
    margin-top: 1rem;
  }

  .button:hover {
    background-color: #2563eb;
  }
</style>
