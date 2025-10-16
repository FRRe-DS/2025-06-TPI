<script>
   const shipments = [
     {
       id: 'PAP-00123',
       destination: 'Buenos Aires, Argentina',
       status: 'En Tránsito',
       entryDate: '2025-10-15',
     },
     {
       id: 'PAP-00124',       destination: 'Córdoba, Argentina',
       status: 'Procesando',
       entryDate: '2025-10-16',
     },
     {
       id: 'PAP-00125',
       destination: 'Rosario, Argentina',
       status: 'Entregado',
       entryDate: '2025-10-12',
     },
   ];
 </script>

 <h3>Listado de Pedidos</h3>

 <table>
   <thead>
     <tr>
       <th>ID Pedido</th>
       <th>Destino</th>
       <th>Estado</th>
       <th>Fecha Ingreso</th>
     </tr>
   </thead>
   <tbody>
     {#each shipments as shipment}
       <tr>
         <td>{shipment.id}</td>
         <td>{shipment.destination}</td>
         <td>{shipment.status}</td>
         <td>{shipment.entryDate}</td>
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
     border: 1px solid #ddd;
     padding: 8px;
     text-align: left;
   }

   th {
    background-color: #f2f2f2;
}
</style>