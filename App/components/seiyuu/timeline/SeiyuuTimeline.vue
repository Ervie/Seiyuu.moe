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
        <span slot="opposite" class="title"> {{ anime.aired_date }} </span>
        <v-card>
          <v-layout align-center justify-center row fill-height v-if="i % 2 === 1">
            <v-flex xs8>
              <div class="headline"> {{ anime.name }} </div>
            </v-flex>
            <v-flex xs4>
              <v-layout align-center justify-center row fill-height>
                <v-img :src="pathToImage(anime.image_url)" class="timeline-image"/>
              </v-layout>
            </v-flex>
          </v-layout>
          <v-layout align-center justify-center row fill-height v-else>
            <v-flex xs4>
              <v-layout align-center justify-center>
               <v-img :src="pathToImage(anime.image_url)" class="timeline-image"/>
              </v-layout>
            </v-flex>
            <v-flex xs8>
              <div class="headline"> {{ anime.name }} </div>
            </v-flex>
          </v-layout>
        </v-card>
      </v-timeline-item>
    </v-timeline>
  </v-container>
  <v-container hidden-lg-and-up>
    <v-timeline
      align-top
      dense>
      <v-timeline-item
        v-for="(anime,i) in items"
        :key="i"
        color="accent"
      >
        <v-layout align-center justify-center row fill-height >
            <v-flex xs4>
        <div class="caption" > {{ anime.aired_date }} </div>
            </v-flex>
            <v-flex xs8>
        <div class="subheading"> {{ anime.name }}  </div>
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


