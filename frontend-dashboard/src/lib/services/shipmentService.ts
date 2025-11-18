import type { DashboardShipmentDto, PaginatedDashboardShipmentsResponse, ShippingDetail, Locality, FiltersState } from '$lib/types';
import { PUBLIC_BACKEND_API_KEY } from '$env/static/public'; // Keep this if PUBLIC_BACKEND_API_KEY is defined elsewhere
import { browser } from '$app/environment'; // Import 'browser'

// --- 1. CONFIGURACIÓN DE URLS ---
// Tu lógica para la URL de la API (¡esto está perfecto!)
const API_BASE_URL = browser ? import.meta.env.VITE_PUBLIC_API_URL : import.meta.env.VITE_PRIVATE_API_URL;

// Configuración de Keycloak (como corre en el navegador, 'localhost' está bien)
const KEYCLOAK_URL = "http://localhost:8080/realms/ds-2025-realm/protocol/openid-connect/token";
const CLIENT_ID = "grupo-06";
const CLIENT_SECRET = "8dc00e75-ccea-4d1a-be3d-b586733e256c"; // El secreto que ya descubrimos

// --- 2. FUNCIÓN PARA OBTENER EL TOKEN (LA LLAVE) ---
async function getAuthToken(): Promise<string> {
    const body = new URLSearchParams();
    body.append("grant_type", "client_credentials");
    body.append("client_id", CLIENT_ID);
    body.append("client_secret", CLIENT_SECRET);

    try {
        const response = await fetch(KEYCLOAK_URL, {
            method: "POST",
            headers: { "Content-Type": "application/x-www-form-urlencoded" },
            body: body,
        });

        if (!response.ok) {
            throw new Error(`Error al obtener token de Keycloak: ${response.statusText}`);
        }

        const data = await response.json();
        return data.access_token;
    } catch (error) {
        console.error("Fallo grave en autenticación:", error);
        throw error; // Detenemos la ejecución si no hay token
    }
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

// --- 3. FUNCIÓN DE DASHBOARD (MODIFICADA) ---
export async function getDashboardShipments(page: number = 1, pageSize: number = 10, filters: FiltersState): Promise<PaginatedDashboardShipmentsResponse> {
    const params = new URLSearchParams({
        page: page.toString(),
        pageSize: pageSize.toString(),
    });

    if (filters.id) params.append('id', filters.id);
    if (filters.city) params.append('city', filters.city);
    if (filters.status) params.append('status', filters.status);
    if (filters.startDate) params.append('startDate', filters.startDate);
    if (filters.endDate) params.append('endDate', filters.endDate);

    const url = `${API_BASE_URL}/dashboard/shipments?${params.toString()}`;

    try {
        // A. Primero, conseguimos la llave
        const token = await getAuthToken();

        // B. Segundo, llamamos a la API con la llave
        const response = await fetch(url, {
            headers: {
                // Reemplazamos 'X-Internal-API-Key' por la autenticación correcta
                'Authorization': `Bearer ${token}`
            },
        });

        if (!response.ok) {
             if (response.status === 401 || response.status === 403) {
                console.error("¡Error de Autenticación! El token fue rechazado por la API.", response.statusText);
             }
            throw new Error(`Failed to fetch dashboard shipments: ${response.statusText}`);
        }
        return await response.json();

    } catch (error) {
        console.error("Error en getDashboardShipments:", error);
        throw error; // Dejamos que el componente Svelte maneje el error
    }
}

// Esta función ahora usa automáticamente la nueva autenticación
export async function getAllShipments(page: number = 1, page_size: number = 10, filters: FiltersState): Promise<PaginatedDashboardShipmentsResponse> {
    return getDashboardShipments(page, page_size, filters);
}

// Esta función es PÚBLICA (según la guía del profe), así que la dejamos como estaba.
export async function getShipmentById(id: string): Promise<ShippingDetail | undefined> {
    const response = await fetch(`${API_BASE_URL}/shipping/${id}`); // Sin token
    if (!response.ok) {
        if (response.status === 404) {
            return undefined; // Shipment not found
        }
        throw new Error(`Failed to fetch shipment ${id}: ${response.statusText}`);
    }
    const detailData: ShippingDetail = await response.json();
    return detailData;
}
