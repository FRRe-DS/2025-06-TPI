import { getShipmentStatusDistribution } from '$lib/services/shipmentService';
import type { PageLoad } from './$types';

export const load: PageLoad = async ({ fetch }) => {
  try {
    const statusDistribution = await getShipmentStatusDistribution(fetch);
    return {
      statusDistribution,
    };
  } catch (error) {
    console.error('Error loading shipment status distribution:', error);
    return {
      statusDistribution: [],
      error: 'Could not load shipment status distribution.',
    };
  }
};
