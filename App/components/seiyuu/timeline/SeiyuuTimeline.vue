<template>
<div>
  <v-container hidden-md-and-down>
    <v-timeline v-if="items != null">
      <v-timeline-item
      color="secondary"
      fill-dot
      >
        <v-btn 
          flat icon
          @click="orderFromEldest = !orderFromEldest"
          slot="icon">
          <font-awesome-icon v-if="orderFromEldest"  size="2x" :icon="['fas', 'arrow-circle-up']"/>
          <font-awesome-icon v-else size="2x" :icon="['fas', 'arrow-circle-down']"/>
        </v-btn>
      </v-timeline-item>
      <v-timeline-item
        v-for="(anime,i) in timelineData"
        :key="i"
        color="accent"
      >
        <span slot="opposite" class="title"> {{ anime.airedDate }} </span>
        <v-card>
          <v-layout align-center justify-center row fill-height v-if="i % 2 === 1">
            <v-flex xs8>
              <div class="headline"> {{ anime.title }} </div>
            </v-flex>
            <v-flex xs4>
              <v-layout align-center justify-center row fill-height>
                <v-img :src="pathToImage(anime.imageUrl)" class="timeline-image"/>
              </v-layout>
            </v-flex>
          </v-layout>
          <v-layout align-center justify-center row fill-height v-else>
            <v-flex xs4>
              <v-layout align-center justify-center>
               <v-img :src="pathToImage(anime.imageUrl)" class="timeline-image"/>
              </v-layout>
            </v-flex>
            <v-flex xs8>
              <div class="headline"> {{ anime.title }} </div>
            </v-flex>
          </v-layout>
        </v-card>
      </v-timeline-item>
    </v-timeline>
  </v-container>
  <v-container hidden-lg-and-up>
    <v-timeline
      dense>
      <v-timeline-item
      color="secondary"
      small
      fill-dot
      >
        <v-btn 
          flat icon
          @click="orderFromEldest = !orderFromEldest"
          slot="icon">
          <font-awesome-icon v-if="orderFromEldest" :icon="['fas', 'arrow-circle-up']"/>
          <font-awesome-icon v-else :icon="['fas', 'arrow-circle-down']"/>
        </v-btn>
      </v-timeline-item>
      <v-timeline-item
        v-for="(anime,i) in timelineData"
        :key="i"
        color="accent"
        small
      >
        <v-layout align-center justify-center row fill-height >
            <v-flex xs4>
        <div class="caption" > {{ anime.airedDate }} </div>
            </v-flex>
            <v-flex xs8>
        <div class="subheading"> {{ anime.title }}  </div>
            </v-flex>
        </v-layout>
      </v-timeline-item>
    </v-timeline>
  </v-container>
</div>
</template>

<script>
export default {
    name: 'SeiyuuTimeline',
    props: {
        items: {
          type: Array,
          required: false,
          default: () => {
            return [];
          }
        }
    },
    data() {
      return {
        orderFromEldest: true
      }
    },
    computed: {
      timelineData() {
        return this.orderFromEldest ? this.items : this.items.slice().reverse(); 
      }
    }
}
</script>

<style>
  .timeline-dot {
    width: 144px;
    height: 144px;
  }

  .timeline-image {
    width: 145px;
    height: 210px;
    flex: 0 0 auto;
  }
</style>


