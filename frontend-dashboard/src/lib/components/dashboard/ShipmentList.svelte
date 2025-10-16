<script>
   import { onMount } from 'svelte';
   import { getShipments } from '../../services/shipmentService';

   let shipments = [];

   const statusNames = {
     'created': 'Creado',
     'reserved': 'Reservado',
     'in_transit': 'En Tránsito',
     'delivered': 'Entregado',
     'cancelled': 'Cancelado',
     'in_distribution': 'En Distribución',
     'arrived': 'Arribado',
   };

   const statusColors = {
     'created': '#f0e68c', // Amarillo caqui
     'reserved': '#ffa07a', // Salmón claro
     'in_transit': '#add8e6', // Azul claro
     'delivered': '#98fb98', // Verde pálido
     'cancelled': '#f08080', // Coral claro
     'in_distribution': '#dda0dd', // Ciruela
     'arrived': '#8fbc8f', // Verde mar oscuro
   };

   function getColorForStatus(status) {
     if (statusColors[status]) {
       return statusColors[status];
     }
     let hash = 0;
     for (let i = 0; i < status.length; i++) {
       hash += status.charCodeAt(i);
     }
     const colors = ['#ff6347', '#9370db', '#f08080']; // Fallback colors
     return colors[hash % colors.length];
   }

   onMount(async () => {
     shipments = await getShipments();
   });
 </script>

 <h3>Listado de Pedidos</h3>

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
         <td>{shipment.id}</td>
         <td>{shipment.destination}</td>
         <td>
            <span class="status-circle" style="background-color: {getColorForStatus(shipment.status)}"></span>
            {statusNames[shipment.status] || shipment.status}
         </td>
         <td>{shipment.entryDate}</td>
         <td><a href="/shipments/{shipment.id}" class="button">Ver Detalles</a></td>
       </tr>
     {/each}
   </tbody>
 </table>

 <style>
   table {
     width: 100%;
     border-collapse: collapse;
     margin-top: 1rem;
   }

   th, td {
     border: 1px solid #444;
     padding: 8px;
     text-align: left;
   }

   th {
    background-color: #2a2a2a;
   }

   button {
    background-color: #3b82f6;
    color: white;
    border: none;
    padding: 8px 12px;
    border-radius: 4px;
    cursor: pointer;
   }

   button:hover, a.button:hover {
    background-color: #2563eb;
   }

   a.button {
    background-color: #3b82f6;
    color: white;
    border: none;
    padding: 4px 8px;
    border-radius: 4px;
    cursor: pointer;
    text-decoration: none;
   }

   .status-circle {
     display: inline-block;
     width: 10px;
     height: 10px;
     border-radius: 50%;
     margin-right: 8px;
   }
</style>