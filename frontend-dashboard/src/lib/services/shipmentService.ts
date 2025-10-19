// src/lib/services/shipmentService.ts
import type { Shipment } from '$lib/types';

const mockShipments: Shipment[] = [
  // Fechas corregidas al formato estándar
  { id: 'PAP-00123', destination: 'Buenos Aires, Argentina', status: 'in_transit', entryDate: '2025-10-15' },
  { id: 'PAP-00124', destination: 'Córdoba, Argentina', status: 'created', entryDate: '2025-10-16' },
  { id: 'PAP-00125', destination: 'Rosario, Argentina', status: 'delivered', entryDate: '2025-10-11' },
  { id: 'PAP-00126', destination: 'Mendoza, Argentina', status: 'cancelled', entryDate: '2025-10-11' },
  { id: 'PAP-00127', destination: 'Tucumán, Argentina', status: 'in_distribution', entryDate: '2025-10-17' },
  { id: 'PAP-00128', destination: 'Salta, Argentina', status: 'arrived', entryDate: '2025-10-18' },
  { id: 'PAP-00129', destination: 'La Plata, Argentina', status: 'reserved', entryDate: '2025-10-19' }
];

export async function getAllShipments(): Promise<Shipment[]> {
  return mockShipments;
}

export async function getShipmentById(id: string): Promise<Shipment | undefined> {
  return mockShipments.find(s => s.id === id);
}

// ... resto del archivo