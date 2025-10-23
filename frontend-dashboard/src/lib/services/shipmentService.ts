import type { Shipment } from '$lib/types';

export async function getAllShipments(): Promise<Shipment[]> {
  const res = await fetch('/api/shipments');
  if (!res.ok) throw new Error('Error al obtener envíos');
  return await res.json();
}

export async function getShipmentById(id: string): Promise<Shipment | undefined> {
  const res = await fetch(`/api/shipments/${id}`);
  if (!res.ok) throw new Error('Envío no encontrado');
  return await res.json();
}
