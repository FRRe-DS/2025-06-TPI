export type ShipmentStatus =
  | 'in_transit'
  | 'created'
  | 'delivered'
  | 'cancelled'
  | 'in_distribution'
  | 'arrived'
  | 'reserved';

export interface Shipment {
  id: string;
  destination: string;
  status: ShipmentStatus;
  entryDate: string; // Formato 'YYYY-MM-DD'
}

export interface PaginationData {
  current_page: number;
  total_pages: number;
  total_items: number;
  items_per_page: number;
}

export interface PaginatedShipmentsResponse {
  shipments: Shipment[];
  pagination: PaginationData;
}

/**
 * Define la estructura del objeto que maneja el estado de los filtros.
 */
export interface FiltersState {
  id: string; // Para el buscador por ID
  city: string;
  status: ShipmentStatus | '';
  startDate: string; // Para el filtro por fecha
  endDate: string;   // Para el filtro por fecha
}