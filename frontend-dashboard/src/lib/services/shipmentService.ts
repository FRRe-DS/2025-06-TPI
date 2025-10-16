import { json } from '@sveltejs/kit';

// Mock data
const shipments = [
  {
    id: 'PAP-00123',
    destination: 'Buenos Aires, Argentina',
    status: 'in_transit',
    entryDate: '2025-10-15',
  },
  {
    id: 'PAP-00124',
    destination: 'Córdoba, Argentina',
    status: 'created',
    entryDate: '2025-10-16',
  },
  {
    id: 'PAP-00125',
    destination: 'Rosario, Argentina',
    status: 'delivered',
    entryDate: '2025-10-12',
  },
  {
    id: 'PAP-00126',
    destination: 'Mendoza, Argentina',
    status: 'cancelled',
    entryDate: '2025-10-11',
  },
  {
    id: 'PAP-00127',
    destination: 'Tucumán, Argentina',
    status: 'in_distribution',
    entryDate: '2025-10-17',
  },
  {
    id: 'PAP-00128',
    destination: 'Salta, Argentina',
    status: 'arrived',
    entryDate: '2025-10-18',
  },
  {
    id: 'PAP-00129',
    destination: 'La Plata, Argentina',
    status: 'reserved',
    entryDate: '2025-10-19',
  },
];

const shipmentDetails = {
  'PAP-00123': {
    id: 'PAP-00123',
    origin: 'Depósito Central, Resistencia, Chaco',
    destination: 'Av. Siempre Viva 123, Springfield',
    status: 'En Tránsito',
    estimatedDelivery: '2025-10-20',
    history: [
      { date: '2025-10-15', status: 'Creado', description: 'Pedido recibido en el depósito' },
      { date: '2025-10-16', status: 'En Tránsito', description: 'El paquete ha salido del depósito' },
    ],
    products: [
      { name: 'Producto A', quantity: 2 },
      { name: 'Producto B', quantity: 1 },
    ],
  }
  // Add more details for other shipments if needed
};

export const getShipments = async () => {
  // In the future, this will fetch from the real API
  return shipments;
};

export const getShipmentById = async (id) => {
  // In the future, this will fetch from the real API
  // @ts-ignore
  return shipmentDetails[id] || { id, error: 'Not Found' };
};
