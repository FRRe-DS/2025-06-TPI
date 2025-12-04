import { getShipmentStatusDistribution } from '$lib/services/shipmentService';
import type { PageLoad } from './$types';

export const load: PageLoad = async ({ fetch, url }) => {
  const limit = url.searchParams.get('limit');
  const limitNumber = limit ? parseInt(limit, 10) : null;

  try {
    const statusDistribution = await getShipmentStatusDistribution(fetch, limitNumber);
    return {
      statusDistribution,
      limit: limitNumber,
    };
  } catch (error) {
    console.error('Error loading shipment status distribution:', error);
    return {
      statusDistribution: [],
      limit: limitNumber,
      error: 'Could not load shipment status distribution.',
    };
  }
};
