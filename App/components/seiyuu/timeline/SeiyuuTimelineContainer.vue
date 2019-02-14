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
        },
        showTimeline: {
            type: Boolean,
            required: true,
            default: false
        }
    },
    data() {
        return {
            dates: [],
            outputData: []
        }
    },
    methods: {
        getTimelineDates() {
            var malIds = this.timelineItems.map(x => x.anime.mal_id).join('&malId=');

            this.$axios.get(process.env.API_URL + '/api/Anime/AiringDates?malId=' + malIds)
                .then((response) => {
                    this.dates = response.data;
                    this.matchDatesWithAnime();
                })
                .catch((error) => {
                    console.log(error);
                });
            },
        formatDate(inputDate) {
            var m = new Date(inputDate);
            return m.getUTCFullYear() + "." +
                ("0" + (m.getUTCMonth()+1)).slice(-2) + "." +
                ("0" + m.getUTCDate()).slice(-2);
        },
        matchDatesWithAnime() {
            var tempData = [];
            
            this.timelineItems.map(x => x.anime).forEach(anime => {
                if (!tempData.map(x => x.mal_id).includes(anime.mal_id)) {
                    tempData.push(anime);
                }
            });
            console.log(tempData)
            tempData.forEach(anime => {
                anime.aired_date = this.formatDate(this.dates[this.dates.map(x => x.mal_id).indexOf(anime.mal_id)].airing_from);
            });
            
            this.outputData = tempData.sort((a,b) => (a.aired_date > b.aired_date) ? 1 : ((b.aired_date > a.aired_date) ? -1 : 0));
        }
    },
    watch: {
        timelineItems: {
            handler: 'getTimelineDates',
            immediate: false
        }
    }

}
</script>

<style scoped>

</style>