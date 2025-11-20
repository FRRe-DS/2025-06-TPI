<script lang="ts">
  import type { DashboardShipmentDto, ShipmentStatus } from '$lib/types';

  export let shipments: DashboardShipmentDto[] = [];

  const statusNames: Record<ShipmentStatus, string> = {
    'created': 'Creado',
    'reserved': 'Reservado',
    'in_transit': 'En Tránsito',
    'delivered': 'Entregado',
    'cancelled': 'Cancelado',
    'in_distribution': 'En Distribución',
    'arrived': 'Arribado',
  };

  const statusColors: Record<ShipmentStatus, string> = {
    'created': '#f0e68c',
    'reserved': '#ffa07a',
    'in_transit': '#add8e6',
    'delivered': '#98fb98',
    'cancelled': '#f08080',
    'in_distribution': '#dda0dd',
    'arrived': '#8fbc8f',
  };

  function hexToRgba(hex: string, opacity = 1): string {
    if (!/^#([A-Fa-f0-9]{3}){1,2}$/.test(hex)) {
        return 'rgba(0,0,0,0)';
    }
    let c: any = hex.substring(1).split('');
    if (c.length === 3) {
        c = [c[0], c[0], c[1], c[1], c[2], c[2]];
    }
    c = '0x' + c.join('');
    return `rgba(${[(c>>16)&255, (c>>8)&255, c&255].join(',')},${opacity})`;
  }

  function getHighlightColor(hex: string, status: ShipmentStatus): string {
    if (!/^#([A-Fa-f0-9]{3}){1,2}$/.test(hex)) {
        return '#000000'; // default to black
    }

    // hex to rgb
    let c: any = hex.substring(1).split('');
    if (c.length === 3) {
        c = [c[0], c[0], c[1], c[1], c[2], c[2]];
    }
    c = '0x' + c.join('');
    let r = (c >> 16) & 255;
    let g = (c >> 8) & 255;
    let b = c & 255;

    // rgb to hsl
    r /= 255, g /= 255, b /= 255;
    const max = Math.max(r, g, b), min = Math.min(r, g, b);
    let h: number = 0, s: number, l: number = (max + min) / 2;

    if (max === min) {
        h = s = 0; // achromatic
    } else {
        const d = max - min;
        s = l > 0.5 ? d / (2 - max - min) : d / (max + min);
        switch (max) {
            case r: h = (g - b) / d + (g < b ? 6 : 0); break;
            case g: h = (b - r) / d + 2; break;
            case b: h = (r - g) / d + 4; break;
        }
        h /= 6;
    }

    // adjust lightness
    let newLightness;
    if (status === 'created' || status === 'reserved' || status === 'delivered' || status === 'in_transit') {
        newLightness = 0.1; // Very dark for these specific statuses
    } else {
        newLightness = l > 0.55 ? l - 0.5 : l + 0.5;
    }

    // hsl to rgb
    if (s === 0) {
        r = g = b = newLightness; // achromatic
    } else {
        const hue2rgb = (p: number, q: number, t: number) => {
            if (t < 0) t += 1;
            if (t > 1) t -= 1;
            if (t < 1/6) return p + (q - p) * 6 * t;
            if (t < 1/2) return q;
            if (t < 2/3) return p + (q - p) * (2/3 - t) * 6;
            return p;
        };
        const q = newLightness < 0.5 ? newLightness * (1 + s) : newLightness + s - newLightness * s;
        const p = 2 * newLightness - q;
        r = hue2rgb(p, q, h + 1/3);
        g = hue2rgb(p, q, h);
        b = hue2rgb(p, q, h - 1/3);
    }

    r = Math.round(r * 255);
    g = Math.round(g * 255);
    b = Math.round(b * 255);

    // rgb to hex
    return "#" + ((1 << 24) + (r << 16) + (g << 8) + b).toString(16).slice(1);
  }

  /**
   * TOMA UNA FECHA ISO STRING Y LA DEVUELVE COMO 'DD/MM/YYYY'.
   */
  function formatDate(dateString: string): string {
    if (!dateString) return '';
    const date = new Date(dateString);
    const day = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0'); // Months are 0-indexed
    const year = date.getFullYear();
    return `${day}/${month}/${year}`;
  }
</script>

<table>
  <thead>
    <tr>
      <th>ID Pedido</th>
      <th>Destino</th>
      <th>Estado</th>
      <th>Fecha Ingreso</th>
      <th>Acciones</th>
    </tr>
  </thead>
  <tbody>
    {#each shipments as shipment}
      <tr>
        <td>{shipment.shipping_id}</td>
        <td>{shipment.delivery_address.locality_name}</td>
        <td>
          <span
            class="status-pill"
            style="background-color: {hexToRgba(statusColors[shipment.status] || '#ccc', 0.7)}; color: {getHighlightColor(statusColors[shipment.status] || '#ccc', shipment.status)}"
          >
            {statusNames[shipment.status] || shipment.status}
          </span>
        </td>
        <td>{formatDate(shipment.created_at)}</td>
        <td><a href="/shipments/{shipment.shipping_id}" class="button">Ver Detalles</a></td>
      </tr>
    {:else}
      <tr>
        <td colspan="5">No se encontraron pedidos con los filtros seleccionados.</td>
      </tr>
    {/each}
  </tbody>
</table>

<style>
  /* ... tus estilos de la tabla aquí ... */
  table {
    width: 100%;
    border-collapse: collapse;
    margin-top: 1rem;
  }
  th, td {
    border: 1px solid var(--border);
    padding: 8px;
    text-align: left;
  }
  th {
    background-color: var(--card);
  }
  .status-pill {
    padding: 4px 12px;
    border-radius: 9999px; /* large value to ensure pill shape */
    font-weight: bold;
    display: inline-block;
    text-align: center;
    white-space: nowrap;
  }
  a.button {
    background-color: var(--accent);
    color: white;
    padding: 4px 8px;
    border-radius: 4px;
    text-decoration: none;
  }
  a.button:hover {
    background-color: #2563eb; /* keep a slightly darker accent on hover */
  }
</style>