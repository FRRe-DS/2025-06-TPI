// src/lib/services/shipmentService.ts
import type { Shipment } from '$lib/types';

export async function getAllShipments(page: number = 1, page_size: number = 10): Promise<PaginatedShipmentsResponse> {
  const url = `/api/shipping?page=${page}&page_size=${page_size}`;
  const response = await fetch(url);
  if (!response.ok) {
    throw new Error(`Failed to fetch shipments: ${response.statusText}`);
  }
  const data = await response.json();
  const backendShipments = data.shipments;
  const pagination = data.pagination;

  const shipments: Shipment[] = await Promise.all(
    backendShipments.map(async (backendShipment: any) => {
      const detailResponse = await fetch(`/api/shipping/${backendShipment.shipping_id}`);
      if (!detailResponse.ok) {
        console.error(`Failed to fetch detail for shipment ${backendShipment.shipping_id}: ${detailResponse.statusText}`);
        return null; // Or handle error as appropriate
      }
      const detailData = await detailResponse.json();
      const destination = detailData.delivery_address?.locality_name || 'Unknown';

      return {
        id: backendShipment.shipping_id.toString(),
        destination: destination,
        status: backendShipment.status.toLowerCase(), // Assuming status names match
        entryDate: new Date(backendShipment.created_at).toISOString().split('T')[0] // Format to YYYY-MM-DD
      };
    })
  );

  return {
    shipments: shipments.filter(s => s !== null) as Shipment[],
    pagination: pagination
  };
}

export async function getShipmentById(id: string): Promise<Shipment | undefined> {
  const response = await fetch(`/api/shipping/${id}`);
  if (!response.ok) {
    if (response.status === 404) {
      return undefined; // Shipment not found
    }
    throw new Error(`Failed to fetch shipment ${id}: ${response.statusText}`);
  }
  const detailData = await response.json();
  const destination = detailData.delivery_address?.locality_name || 'Unknown';

  return {
    id: detailData.shipping_id.toString(),
    destination: destination,
    status: detailData.status.toLowerCase(),
    entryDate: new Date(detailData.created_at).toISOString().split('T')[0]
  };
}