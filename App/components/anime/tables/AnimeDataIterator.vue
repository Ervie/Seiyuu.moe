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
            class="primary title font-weight-bold mobile-card-text"> 
            {{ swapNameSurname(decodeHtml(seiyuuEntry.name))}}
            </v-card-text>
          <v-container fluid grid-list-xs class="smaller-padding" v-for="(role, i) in props.item.animeCharacters" :key="'i' + i">
            <v-card-text class="title mobile-card-text"> {{ role.anime.title}} </v-card-text>
            <v-layout justify-center row wrap>
              <v-flex xs6 sm4 md3
                v-for="character in role.characters" 
                :key="character.malId"> 
                <div class="body-2">
                  <v-img :src="pathToImage(character.imageUrl)" />
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

<style scoped>
.mobile-card-text {
  padding: 8px;
}
</style>