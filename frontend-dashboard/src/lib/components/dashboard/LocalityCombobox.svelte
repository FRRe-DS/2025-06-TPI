<script lang="ts">
  import { createEventDispatcher, onMount } from 'svelte';
  import { searchLocalities, getAllLocalities } from '$lib/services/shipmentService';
  import type { Locality } from '$lib/types';

  export let selectedLocality: Locality | undefined = undefined;

  let inputValue = '';
  let searchResults: Locality[] = [];
  // Caché local de todas las localidades (precargadas al montar)
  let cachedAllLocalities: Locality[] | null = null;
  let isDropdownVisible = false;
  let isLoading = false;
  let debounceTimer: any;

  // --- ESTADOS PARA PAGINACIÓN ---
  let currentPage = 1;
  let hasMore = true;
  const PAGE_SIZE = 20;

  const dispatch = createEventDispatcher();

  async function performSearch(isPaginating = false) {
    // Esta función sigue usando la búsqueda del servidor para consultas "completas" (>=2 caracteres).
    if (inputValue.length < 2) {
      return;
    }

    if (!isPaginating) {
      currentPage = 1;
      searchResults = [];
      hasMore = true;
    }
    
    isLoading = true;
    try {
      const newResults = await searchLocalities(inputValue, currentPage);
      // ordenar alfabéticamente los nuevos resultados
      newResults.sort((a, b) => a.locality_name.localeCompare(b.locality_name, 'es', { sensitivity: 'base' }));

      // fusionar y mantener orden alfabético
      searchResults = [...searchResults, ...newResults];
      searchResults.sort((a, b) => a.locality_name.localeCompare(b.locality_name, 'es', { sensitivity: 'base' }));

      if (newResults.length < PAGE_SIZE) {
        hasMore = false;
      }

    } catch (error) {
      console.error('Error al buscar localidades:', error);
      searchResults = [];
    } finally {
      isLoading = false;
    }
  }

  function onInput() {
    isDropdownVisible = true;
    clearTimeout(debounceTimer);
    // Si no hay texto, mostramos la lista precargada (si existe)
    if (!inputValue) {
      if (cachedAllLocalities) {
        searchResults = [...cachedAllLocalities];
        isDropdownVisible = true;
      } else {
        searchResults = [];
        isDropdownVisible = false;
      }
      return;
    }

    // Si hay cache local, filtramos primero localmente y mostramos coincidencias inmediatas
    if (cachedAllLocalities) {
      const q = inputValue.toLowerCase();
      const localMatches = cachedAllLocalities.filter((l) =>
        l.locality_name.toLowerCase().includes(q),
      );

      if (localMatches.length > 0) {
        // Ordenar y mostrar coincidencias locales rápidamente
        localMatches.sort((a, b) => a.locality_name.localeCompare(b.locality_name, 'es', { sensitivity: 'base' }));
        searchResults = localMatches;
        isDropdownVisible = true;
        // También lanzamos en segundo plano la búsqueda servidor si el query es largo, para obtener más resultados
        if (inputValue.length >= 2) {
          clearTimeout(debounceTimer);
          debounceTimer = setTimeout(() => performSearch(false), 400);
        }
        return;
      }
      // Si no hubo coincidencias locales, dejamos caer al flujo de búsqueda remota
    }

    // Para 2 o más caracteres, llamamos al servidor (con debounce)
    if (inputValue.length >= 2) {
      debounceTimer = setTimeout(() => {
        performSearch(false);
      }, 300);
    } else {
      // Para 1 carácter y sin coincidencias locales, mostramos vacío o mensaje tras debounce corto
      debounceTimer = setTimeout(() => {
        searchResults = [];
      }, 200);
    }
  }

  function selectLocality(locality: Locality) {
    inputValue = locality.locality_name;
    isDropdownVisible = false;
    dispatch('select', { locality: locality });
  }
  
  function handleScroll(event: Event) {
    const list = event.target as HTMLElement;
    const isAtBottom = list.scrollTop + list.clientHeight >= list.scrollHeight - 20;

    if (isAtBottom && hasMore && !isLoading) {
      currentPage++;
      performSearch(true);
    }
  }

  function handleFocus() {
    // Mostrar sugerencias cuando el campo recibe foco (si hay resultados precargados)
    if (cachedAllLocalities && !inputValue) {
      searchResults = [...cachedAllLocalities];
    }
    isDropdownVisible = true;
  }

  function handleBlur() {
    setTimeout(() => {
      isDropdownVisible = false;
    }, 200);
  }

  // Precargar la lista al montar la UI y ordenarla alfabéticamente
  onMount(async () => {
    try {
      isLoading = true;
      const all = await getAllLocalities();
      all.sort((a, b) => a.locality_name.localeCompare(b.locality_name, 'es', { sensitivity: 'base' }));
      cachedAllLocalities = all;
      // Mostrar al inicio la lista completa ordenada
      searchResults = [...all];
      hasMore = false; // la carga inicial viene completa desde el backend
    } catch (err) {
      console.error('No se pudieron precargar las localidades:', err);
      cachedAllLocalities = null;
      searchResults = [];
    } finally {
      isLoading = false;
    }
  });
</script>

<div class="combobox-container">
  <input
    type="text"
    bind:value={inputValue}
    on:input={onInput}
    on:focus={handleFocus}
    on:blur={handleBlur}
    placeholder="Escriba para buscar una localidad..."
  />
  {#if isDropdownVisible}
    <ul class="dropdown" on:scroll={handleScroll}>
      {#if isLoading && searchResults.length === 0}
        <li class="disabled">Buscando...</li>
      {:else if searchResults.length === 0 && inputValue.length >= 2}
        <li class="disabled">No se encontraron resultados.</li>
      {:else}
        {#each searchResults as locality, i (locality.postal_code + locality.locality_name + i)}
          <li>
            <button
              type="button"
              on:mousedown={() => selectLocality(locality)}
              on:keydown={(e) => e.key === 'Enter' && selectLocality(locality)}
            >
              {locality.locality_name} {#if locality.province}({locality.province}){/if}
            </button>
          </li>
        {/each}
        {#if isLoading && searchResults.length > 0}
            <li class="disabled">Cargando más...</li>
        {/if}
      {/if}
    </ul>
  {/if}
</div>

<style>
  .combobox-container {
    position: relative;
    width: 100%;
  }
  input {
    width: 100%;
    padding: 0.75rem;
    border-radius: 4px;
    border: 1px solid #555;
    background-color: var(--card);
    color: var(--text);
    font-size: 1rem;
    box-sizing: border-box;
  }
  .dropdown {
    position: absolute;
    width: 100%;
    list-style: none;
    margin: 4px 0 0 0;
    padding: 0;
    border: 1px solid #555;
    border-radius: 4px;
    background-color: var(--card);
    max-height: 220px;
    overflow-y: auto;
    z-index: 10;
  }
  .dropdown li {
    padding: 0;
  }
  .dropdown li button {
    width: 100%;
    padding: 0.75rem;
    text-align: left;
    cursor: pointer;
    border: none;
    background: transparent;
    color: var(--text);
    font: inherit;
  }
  .dropdown li button:hover {
    background-color: var(--accent);
    color: white;
  }
  .dropdown li.disabled {
    cursor: not-allowed;
    color: #888;
  }
</style>