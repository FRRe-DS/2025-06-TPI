<script lang="ts">
  import { onMount } from 'svelte';
  import { getAllShipments } from '../../services/shipmentService';
  import type { Shipment, FiltersState } from '$lib/types';

  import Filters from './Filters.svelte';
  import ShipmentList from './ShipmentList.svelte';

  let allShipments: Shipment[] = [];
  let filteredShipments: Shipment[] = [];

  onMount(() => {
    loadShipments();
    const interval = setInterval(loadShipments, 10000);
    return () => clearInterval(interval);
  });

  async function loadShipments() {
    try {
      allShipments = await getAllShipments();
      filteredShipments = allShipments;
    } catch (error) {
      console.error('Error al cargar envíos:', error);
    }
  }

  function handleFilterChange(event: CustomEvent<FiltersState>) {
    const { id, city, status, startDate, endDate } = event.detail;
    
    filteredShipments = allShipments.filter(shipment => {
      // 1. Filtro por ID
      const idMatch = id ? shipment.id.toUpperCase().includes(id) : true;

      // 2. Filtro por Ciudad
      const cityMatch = city ? shipment.destination.toLowerCase().includes(city) : true;

      // 3. Filtro por Estado
      const statusMatch = status ? shipment.status === status : true;

      // 4. Filtro por Rango de Fechas
      const entryDate = new Date(shipment.entryDate);
      const start = startDate ? new Date(startDate) : null;
      const end = endDate ? new Date(endDate) : null;
      // Ajustamos las fechas para que la comparación sea inclusiva
      if (start) start.setHours(0, 0, 0, 0);
      if (end) end.setHours(23, 59, 59, 999);

      const dateMatch = (!start || entryDate >= start) && (!end || entryDate <= end);

      // El envío se muestra solo si todas las condiciones son verdaderas
      return idMatch && cityMatch && statusMatch && dateMatch;
    });
  }
</script>

<h2>Dashboard de Pedidos</h2>
<p>Listado de Pedidos</p>

<Filters on:filterChange={handleFilterChange} />

<ShipmentList shipments={filteredShipments} />