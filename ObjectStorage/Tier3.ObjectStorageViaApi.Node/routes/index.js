var AWS = require('aws-sdk');

exports.index = function(req, res){

    AWS.config.loadFromPath('./credentials.json');
    var s3 = new AWS.S3({endpoint:'https://ca.tier3.io'});

    s3.listBuckets(params = {}, function(err, data){

        console.log(data.Buckets);
        res.render('index', { title: 'Tier 3 Object Storage Bucket List', buckets: data.Buckets });

    });

};