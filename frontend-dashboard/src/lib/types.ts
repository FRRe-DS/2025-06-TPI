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