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
            v-for="seiyuuEntry in props.item.seiyuu" 
            :key="seiyuuEntry.entry.mal_id"
            class="primary title font-weight-bold"> 
            {{ swapNameSurname(decodeHtml(seiyuuEntry.entry.name))}}
            </v-card-text>
          <v-container fluid grid-list-lg>
            <v-layout 
              v-for="(role, i) in props.item.roles" 
              :key="'i' + i"
              row>
              <v-flex xs6
                class="body-2">
                {{ role.anime }}
              </v-flex>
              <v-flex xs6>
                <div
                class="body-2"  
                v-for="character in role.characters" 
                :key="character.entry.mal_id">
                  {{ swapNameSurname(decodeHtml(character.entry.name)) }}
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
    name: 'AnimeDataIterator',
    props: {
        items: {
            type: Array,
            required: false
        }
    }
}
</script>