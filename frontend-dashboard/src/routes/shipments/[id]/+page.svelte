<script>
  import { page } from '$app/stores';
  import { getShipmentById } from '../../../lib/services/shipmentService';

  const shipmentId = $page.params.id;
  let shipmentDetails;

  getShipmentById(shipmentId).then(details => {
    shipmentDetails = details;
  });
</script>

{#if shipmentDetails}
<div class="details-container">
  <h2>Detalles del Pedido {shipmentDetails.id}</h2>

  <div class="details-section">
    <h3>Información General</h3>
    <p><strong>Origen:</strong> {shipmentDetails.origin}</p>
    <p><strong>Destino:</strong> {shipmentDetails.destination}</p>
    <p><strong>Estado Actual:</strong> {shipmentDetails.status}</p>
    <p><strong>Fecha Estimada de Entrega:</strong> {shipmentDetails.estimatedDelivery}</p>
  </div>

  <div class="details-section">
    <h3>Historial de Estados</h3>
    <ul>
      {#each shipmentDetails.history as event}
        <li><strong>{event.date}:</strong> {event.status} - {event.description}</li>
      {/each}
    </ul>
  </div>

  <div class="details-section">
    <h3>Productos</h3>
    <ul>
      {#each shipmentDetails.products as product}
        <li>{product.name} (Cantidad: {product.quantity})</li>
      {/each}
    </ul>
  </div>

  <a href="/" class="button">Volver al Listado</a>
</div>
{:else}
<p>Cargando...</p>
{/if}

<style>
  .details-container {
    padding: 2rem;
    background-color: #1e1e1e;
    border-radius: 8px;
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
