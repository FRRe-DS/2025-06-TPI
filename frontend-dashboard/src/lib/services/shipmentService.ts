import type { DashboardShipmentDto, PaginatedDashboardShipmentsResponse, ShippingDetail, Locality } from '$lib/types';
import { PUBLIC_BACKEND_API_KEY } from '$env/static/public'; // Keep this if PUBLIC_BACKEND_API_KEY is defined elsewhere
import { browser } from '$app/environment'; // Import 'browser'

// Conditionally set API_BASE_URL based on environment
// Access variables via import.meta.env
const API_BASE_URL = browser ? import.meta.env.VITE_PUBLIC_API_URL : import.meta.env.VITE_PRIVATE_API_URL;

export async function getDashboardShipments(page: number = 1, pageSize: number = 10): Promise<PaginatedDashboardShipmentsResponse> {
  const url = `${API_BASE_URL}/dashboard/shipments?page=${page}&pageSize=${pageSize}`;
  const response = await fetch(url, {
    headers: {
      'X-Internal-API-Key': PUBLIC_BACKEND_API_KEY,
    },
  });

  if (!response.ok) {
    throw new Error(`Failed to fetch dashboard shipments: ${response.statusText}`);
  }
  return await response.json();
}

// This function will now use the new dashboard endpoint for fetching all shipments
export async function getAllShipments(page: number = 1, page_size: number = 10): Promise<PaginatedDashboardShipmentsResponse> {
  return getDashboardShipments(page, page_size);
}

export async function getShipmentById(id: string): Promise<ShippingDetail | undefined> {
  const response = await fetch(`${API_BASE_URL}/shipping/${id}`);
  if (!response.ok) {
    if (response.status === 404) {
      return undefined; // Shipment not found
    }
    throw new Error(`Failed to fetch shipment ${id}: ${response.statusText}`);
  }
  const detailData: ShippingDetail = await response.json();
  return detailData;
}

export async function getAllLocalities(): Promise<Locality[]> {
  const response = await fetch(`${API_BASE_URL}/locality/getall`);
  if (!response.ok) {
    throw new Error(`Failed to fetch localities: ${response.statusText}`);
  }
  return await response.json();
}

export async function createShipment(shipment: import('$lib/types').CreateShippingRequest): Promise<import('$lib/types').CreateShippingResponse> {
    const response = await fetch(`${API_BASE_URL}/shipping`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'X-Internal-API-Key': PUBLIC_BACKEND_API_KEY,
        },
        body: JSON.stringify(shipment),
    });

    if (!response.ok) {
        throw new Error(`Failed to create shipment: ${response.statusText}`);
    }
    return await response.json();
}