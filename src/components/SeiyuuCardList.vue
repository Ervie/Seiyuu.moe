<template>
<div>
  <v-layout row wrap v-show="seiyuuToCompare.length > 0" hidden-sm-and-down>
      <v-flex v-for="(seiyuu, index) in seiyuuToCompare" :key="seiyuu.mal_id" xs2 class="seiyuuCard"  >
        <seiyuu-card :seiyuuData="seiyuu" :cardId="index" :avatarHeight="280"
         @seiyuuRemoved="removeSeiyuu"/>
      </v-flex>
      <!-- Dummy cards for seiyuu to be filled-->
      <v-flex v-for="i in (maximumSeiyuuNumber - seiyuuToCompare.length)" :key="`emptyLarge${i}`" xs2  class="seiyuuCard">
        <seiyuu-card :cardId="i + seiyuuToCompare.length - 1"/>
      </v-flex>
  </v-layout>
  <v-layout row wrap v-show="seiyuuToCompare.length > 0" hidden-md-and-up>
      <v-flex v-for="(seiyuu, index) in seiyuuToCompare" :key="seiyuu.mal_id" xs4 class="seiyuuCard">
        <seiyuu-card :seiyuuData="seiyuu" :cardId="index" :avatarHeight="210"
         @seiyuuRemoved="removeSeiyuu"/>
      </v-flex>
      <!-- Dummy cards for seiyuu to be filled-->
      <v-flex v-for="i in (maximumSeiyuuNumber - seiyuuToCompare.length)" :key="`emptySmall${i}`" xs4 class="seiyuuCard">
        <seiyuu-card :cardId="i + seiyuuToCompare.length - 1"/>
      </v-flex>
  </v-layout>
  </div>
</template>

<script>
import SeiyuuCard from '@/components/SeiyuuCard.vue'

export default {
  name: 'SeiyuuCardList',
  props: ['seiyuuToCompare', 'maximumSeiyuuNumber'],
  components: {
    'seiyuu-card': SeiyuuCard
  },
  data () {
    return {
      showDialog: false
    }
  },
  methods: {
    removeSeiyuu: function (seiyuuId) {
      console.log('Seiyuu to remove : ' + seiyuuId)
      this.$emit('seiyuuRemoved', seiyuuId)
    }
  }
}
</script>
