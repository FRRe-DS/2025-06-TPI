<script lang="ts">
    import { createShipment } from "../../../../lib/services/shipmentService";
    import type {
        Locality,
        TransportType,
        CreateShippingRequest,
    } from "../../../../lib/types";
    import { goto } from "$app/navigation";
    import LocalityCombobox from "../../../../lib/components/dashboard/LocalityCombobox.svelte";

    let order_id: number;
    let user_id: number;
    let street: string = "";
    let number: number;
    let postal_code: string = "";
    let locality_name: string = "";
    let transport_type: TransportType = "truck";
    let products: { id: number; quantity: number }[] = [{ id: 0, quantity: 1 }];
    let errorMessage: string | null = null;
    let selectedLocality: Locality | undefined;

    function handleLocalitySelect(event: CustomEvent<{ locality: Locality }>) {
        const locality = event.detail.locality;
        selectedLocality = locality;
        locality_name = locality.locality_name;
        postal_code = locality.postal_code;
    }

    function addProduct() {
        products = [...products, { id: 0, quantity: 1 }];
    }

    function removeProduct(index: number) {
        products = products.filter((_, i) => i !== index);
    }

    async function handleSubmit() {
        errorMessage = null;
        if (!selectedLocality) {
            errorMessage = "Por favor, seleccione una localidad.";
            return;
        }
        const newShipment: CreateShippingRequest = {
            order_id,
            user_id,
            delivery_address: {
                street,
                number,
                postal_code,
                locality_name,
            },
            transport_type,
            products,
        };

        try {
            const response = await createShipment(newShipment);
            console.log("Shipment created:", response);
            goto("/dashboard/shipments");
        } catch (error) {
            console.error("Error creating shipment:", error);
            errorMessage =
                "Error al crear el pedido. Verifique los datos e intente nuevamente.";
        }
    }
</script>

<h2>Crear Nuevo Pedido</h2>

{#if errorMessage}
    <p class="error">{errorMessage}</p>
{/if}

<form on:submit|preventDefault={handleSubmit}>
    <div class="form-group">
        <label for="order_id">Order ID</label>
        <input type="number" id="order_id" bind:value={order_id} required />
    </div>

    <div class="form-group">
        <label for="user_id">User ID</label>
        <input type="number" id="user_id" bind:value={user_id} required />
    </div>

    <h3>Dirección de Entrega</h3>
    <div class="form-group">
        <label for="locality">Localidad</label>
        <LocalityCombobox on:select={handleLocalitySelect} />
    </div>
    <div class="form-group">
        <label for="street">Calle</label>
        <input type="text" id="street" bind:value={street} required />
    </div>
    <div class="form-group">
        <label for="number">Número</label>
        <input type="number" id="number" bind:value={number} required />
    </div>
    <div class="form-group">
        <label for="postal_code">Código Postal</label>
        <input type="text" id="postal_code" bind:value={postal_code} readonly />
    </div>

    <div class="form-group">
        <label for="transport_type">Tipo de Transporte</label>
        <select id="transport_type" bind:value={transport_type}>
            <option value="truck">Camión</option>
            <option value="plane">Avión</option>
            <option value="boat">Barco</option>
        </select>
    </div>

    <h3>Productos</h3>
    {#each products as product, i}
        <div class="product-item">
            <div class="form-group">
                <label for="product_id_{i}">Product ID</label>
                <input
                    type="number"
                    id="product_id_{i}"
                    bind:value={product.id}
                    required
                />
            </div>
            <div class="form-group">
                <label for="quantity_{i}">Cantidad</label>
                <input
                    type="number"
                    id="quantity_{i}"
                    bind:value={product.quantity}
                    min="1"
                    required
                />
            </div>
            <button
                type="button"
                on:click={() => removeProduct(i)}
                class="remove-btn">Eliminar</button
            >
        </div>
    {/each}
    <button type="button" on:click={addProduct}>Añadir Producto</button>

    <button type="submit" class="submit-btn">Crear Pedido</button>
</form>

<style>
    form {
        display: flex;
        flex-direction: column;
        gap: 1rem;
        max-width: 600px;
    }
    .form-group {
        display: flex;
        flex-direction: column;
    }
    label {
        margin-bottom: 0.5rem;
        font-weight: bold;
    }
    input,
    select {
        padding: 0.75rem;
        border-radius: 4px;
        border: 1px solid #555;
        background-color: var(--card);
        color: var(--text);
        font-size: 1rem;
    }
    input:read-only {
        background-color: #444;
    }
    h3 {
        margin-top: 1rem;
        margin-bottom: 0;
        border-bottom: 1px solid #444;
        padding-bottom: 0.5rem;
    }
    .product-item {
        display: grid;
        grid-template-columns: 1fr 1fr auto;
        gap: 1rem;
        align-items: flex-end;
        border: 1px solid var(--border);
        padding: 1rem;
        border-radius: 4px;
        background-color: var(--card);
    }
    button {
        padding: 0.75rem 1.5rem;
        border: none;
        border-radius: 4px;
        cursor: pointer;
        font-weight: bold;
        font-size: 1rem;
    }
    .remove-btn {
        background-color: #ef4444;
        color: white;
    }
    .submit-btn {
        background-color: #3b82f6;
        color: white;
        margin-top: 1rem;
    }
    .error {
        color: #ef4444;
        background-color: #ef444420;
        padding: 1rem;
        border-radius: 4px;
    }
</style>
