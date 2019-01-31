<template>
<div>
  <v-layout row wrap v-show="seiyuuToCompare.length > 0" v-if="$vuetify.breakpoint.lgAndUp">
      <v-flex v-for="(seiyuu, index) in seiyuuToCompare" :key="seiyuu.mal_id" xs2 class="seiyuuCard"  >
        <seiyuu-card :seiyuuData="seiyuu" :cardId="index" :avatarHeight="280"
         @seiyuuRemoved="removeSeiyuu"/>
      </v-flex>
  </v-layout>
  <v-layout row wrap v-show="seiyuuToCompare.length > 0" v-if="$vuetify.breakpoint.mdAndDown">
      <v-flex v-for="(seiyuu, index) in seiyuuToCompare" :key="seiyuu.mal_id" xs4 class="seiyuuCard">
        <seiyuu-card :seiyuuData="seiyuu" :cardId="index" :avatarHeight="210"
         @seiyuuRemoved="removeSeiyuu"/>
      </v-flex>
  </v-layout>
  </div>
</template>

<script>
import SeiyuuCard from '@/components/seiyuu/SeiyuuCard.vue'

export default {
  name: 'SeiyuuCardList',
  props: {
    seiyuuToCompare: {
      type: Array,
      required: false
    }
  },
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
      this.$emit('seiyuuRemoved', seiyuuId)
    }
  }
}
</script>
