<template>
  <v-layout row wrap v-show="seiyuuToCompare.length > 0">
      <v-flex v-for="(seiyuu, index) in seiyuuToCompare" :key="seiyuu.mal_id" lg2 xs4 class="seiyuuCard"  >
        <seiyuu-card :seiyuuData="seiyuu" :cardId="index" :avatarHeight="280"
         @seiyuuRemoved="removeSeiyuu"/>
      </v-flex>
  </v-layout>
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
