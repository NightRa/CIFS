package server

import java.util.concurrent.TimeUnit

import io.ipfs.api.Client
import ipfs.Files

import scalaz.concurrent.{Actor, Strategy}

object Republisher {
  val client = new Client("localhost")

  val publishDelayLong = 5000L // 5 seconds - will always publish after 5 seconds if changed
  val publishDelayShort = 1000L // 1 second - will publish 1 sec after the last publish request, if changed
  val naggingRate = 500L // nag the republisher every 500ms
  var lastPublishTime = 0L // epoch time
  var lastPublishRequest = 0L // epoch time
  var lastPublishedHash = ""

  val republisher = new Actor[Boolean](newPublish => {
    if (lastPublishedHash == "") lastPublishedHash = client.resolveSelf().Path
    val currentTime = System.currentTimeMillis()
    if (currentTime - lastPublishTime > publishDelayLong) {
      publish()
    } else if (!newPublish && currentTime - lastPublishRequest > publishDelayShort) {
      publish()
    }
    if (newPublish) {
      lastPublishRequest = currentTime
    }
  }
  )(Strategy.DefaultStrategy)

  // Nag the republisher every 500ms.
  Strategy.DefaultTimeoutScheduler.scheduleAtFixedRate(new Runnable {
    override def run(): Unit = republisher ! false
  }, 0, naggingRate, TimeUnit.MILLISECONDS)

  def publish(): Unit = {
    val hashToPublish = Files.stat("/").get.Hash
    if (lastPublishedHash != hashToPublish) {
      client.publish(hashToPublish)
      lastPublishTime = System.currentTimeMillis()
      lastPublishedHash = hashToPublish
    }
  }
}
