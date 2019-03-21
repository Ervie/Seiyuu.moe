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
            v-for="seiyuuEntry in singleObjectToArray(props.item.seiyuu)" 
            :key="seiyuuEntry.malId"
            class="primary title font-weight-bold"> 
            {{ swapNameSurname(decodeHtml(seiyuuEntry.name))}}
            </v-card-text>
          <v-container fluid grid-list-lg>
            <v-layout 
              v-for="(role, i) in props.item.animeCharacters" 
              :key="'i' + i"
              row>
              <v-flex xs6
                class="body-2">
                {{ role.anime.title }}
              </v-flex>
              <v-flex xs6>
                <div
                class="body-2"  
                v-for="character in role.characters" 
                :key="character.malId">
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
    name: 'AnimeDataIterator',
    props: {
        items: {
            type: Array,
            required: false
        }
    }
}
</script>