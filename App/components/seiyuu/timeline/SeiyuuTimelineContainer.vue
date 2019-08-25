<template>
    <div>
        <seiyuu-timeline v-if="outputData != null"
        :items="outputData" />
    </div>
</template>

<script>
import SeiyuuTimeline from '@/components/seiyuu/timeline/SeiyuuTimeline.vue'

export default {
    name: 'SeiyuuTimelineBase',
    components: {  
        'seiyuu-timeline': SeiyuuTimeline,
    },
    props: {
        timelineItems: {
            type: Array,
            required: false
        }
    },
    computed: {
        outputData() {
            var output = [];
            this.timelineItems.forEach(animeSeries => {
                animeSeries.anime.forEach(anime => {
                    output.push({
                        title: anime.title,
                        imageUrl: anime.imageUrl,
                        airedDate: this.formatDate(anime.airingFrom)
                    });
                });
            });

            return output.sort((a,b) => { return new Date(a.airedDate) < new Date(b.airedDate); });
        }
    }

}
</script>

<style scoped>

</style>