<template>
    <v-data-iterator
      :items="items"
      content-tag="v-layout"
      hide-actions
      row
      wrap
    >
      <v-flex
        slot="item"
        slot-scope="props"
        xs12
      >
        <v-card>
          <v-card-text 
            v-for="animeEntry in singleObjectToArray(props.item.anime)" 
            :key="animeEntry.malIdd"
            class="primary title font-weight-bold"> 
            {{ animeEntry.title }}
            </v-card-text>
          <v-container fluid grid-list-lg>
            <v-layout 
              v-for="role in props.item.roles" 
              :key="role.seiyuu"
              row>
              <v-flex xs6
                class="body-2">
                {{ swapNameSurname(decodeHtml(role.seiyuu.name)) }}
              </v-flex>
              <v-flex xs6>
                <div
                class="body-2"  
                v-for="character in role.characters" 
                :key="character.entry.mal_id">
                  {{ swapNameSurname(decodeHtml(character.name)) }}
                </div>
              </v-flex>
            </v-layout>
          </v-container>
        </v-card>
      </v-flex>
    </v-data-iterator>
</template>

<script>
export default {
    name: 'SeiyuuDataIterator',
    props: {
        items: {
            type: Array,
            required: false
        }
    }
}
</script>