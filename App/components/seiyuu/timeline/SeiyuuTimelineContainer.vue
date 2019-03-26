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

            return output.sort((a,b) => { return new Date(b.airedDate) - new Date(a.airedDate); });
        }
    },
    methods: {
        formatDate(inputDate) {
            var m = new Date(inputDate);
            return m.getUTCFullYear() + "." +
                ("0" + (m.getUTCMonth()+1)).slice(-2) + "." +
                ("0" + m.getUTCDate()).slice(-2);
        }
    }

}
</script>

<style scoped>

</style>