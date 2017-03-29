<?php
//クローラー判定
function isCrawler($clawler=false){
  $ua = $_SERVER['HTTP_USER_AGENT'];

  $crawler_arr = array(
    "Googlebot"     // google
    ,"Baiduspider"  // Baidu
    ,"bingbot"      // Bing
    ,"Yeti"         // NHN
    ,"NaverBot"     // NaverBot
    ,"Yahoo"        // Yahoo
    ,"Tumblr"       // Tumblr
    ,"livedoor"     // livedoor
  );
  foreach ($crawler_arr as $value) {
    if (stripos($ua, $value) !== false) {
        return true;
    }
  }
  return false;
}
?>