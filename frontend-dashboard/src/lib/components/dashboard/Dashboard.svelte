<script lang="ts">
  import { onMount, onDestroy } from 'svelte';
  import { getDashboardShipments } from '../../services/shipmentService';
  import type { DashboardShipmentDto, FiltersState, PaginatedDashboardShipmentsResponse } from '$lib/types';

  import Filters from './Filters.svelte';
  import ShipmentList from './ShipmentList.svelte';

  let allShipments: DashboardShipmentDto[] = [];
  let currentPage: number = 1;
  let totalPages: number = 1;
  let isLoading: boolean = false;
  let hasMore: boolean = true;
  let observer: IntersectionObserver;
  let loadMoreElement: HTMLElement;

  const PAGE_SIZE = 20;

  async function loadShipments() {
    if (isLoading || !hasMore) return;

    isLoading = true;
    try {
      const response: PaginatedDashboardShipmentsResponse = await getDashboardShipments(currentPage, PAGE_SIZE);
      allShipments = [...allShipments, ...response.shipments];
      totalPages = response.pagination.total_pages;
      hasMore = currentPage < totalPages;
      currentPage++;
    } catch (error) {
      console.error('Error loading shipments:', error);
    } finally {
      isLoading = false;
    }
  }

  // Función pura que devuelve una lista filtrada
  function applyFilters(shipments: DashboardShipmentDto[], filters: FiltersState): DashboardShipmentDto[] {
    const { id, city, status, startDate, endDate } = filters;
    
    return shipments.filter(shipment => {
      const idMatch = id ? shipment.shipping_id.toString().toUpperCase().includes(id.toUpperCase()) : true;
      const cityMatch = city && shipment.delivery_address ? shipment.delivery_address.locality_name.toLowerCase().includes(city.toLowerCase()) : true;
      const statusMatch = status ? shipment.status === status : true;
      const entryDate = new Date(shipment.created_at);
      const start = startDate ? new Date(startDate) : null;
      const end = endDate ? new Date(endDate) : null;
      if (start) start.setHours(0, 0, 0, 0);
      if (end) end.setHours(23, 59, 59, 999);
      const dateMatch = (!start || entryDate >= start) && (!end || entryDate <= end);
      return idMatch && cityMatch && statusMatch && dateMatch;
    });
  }

  let currentFilters: FiltersState = {
    id: '',
    city: '',
    status: '',
    startDate: '',
    endDate: ''
  };

  function handleFilterChange(event: CustomEvent<FiltersState>) {
    currentFilters = event.detail;
  }

  onMount(async () => {
    await loadShipments();

    observer = new IntersectionObserver(
      (entries) => {
        const [entry] = entries;
        if (entry.isIntersecting && hasMore && !isLoading) {
          loadShipments();
        }
      },
      { threshold: 0.1 }
    );

    if (loadMoreElement) {
      observer.observe(loadMoreElement);
    }
  });

  onDestroy(() => {
    if (observer) {
      observer.disconnect();
    }
  });

  // Declaración reactiva: filteredShipments se actualizará automáticamente
  // cuando allShipments o currentFilters cambien.
  $: filteredShipments = applyFilters(allShipments, currentFilters);

</script>

<h2>Dashboard de Pedidos</h2>
<p>Listado de Pedidos</p>

<Filters on:filterChange={handleFilterChange} />

<ShipmentList shipments={filteredShipments} />

{#if isLoading}
  <p>Cargando más pedidos...</p>
{:else if !hasMore && allShipments.length > 0}
  <p>No hay más pedidos para cargar.</p>
{/if}

<div bind:this={loadMoreElement} style="height: 1px; margin-top: -1px;"></div>