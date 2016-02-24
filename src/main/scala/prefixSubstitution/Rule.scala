package prefixSubstitution

import com.googlecode.concurrenttrees.common.KeyValuePair
import com.googlecode.concurrenttrees.radix.ConcurrentRadixTree
import com.googlecode.concurrenttrees.radix.ConcurrentRadixTree.SearchResult
import com.googlecode.concurrenttrees.radix.ConcurrentRadixTree.SearchResult.Classification
import com.googlecode.concurrenttrees.radix.node.concrete.SmartArrayBasedNodeFactory

import scala.collection.JavaConversions._

case class Rule(pattern /*x_i*/ : String, replacement /*y_i*/ : String)

object PatternTrie extends App {
  def getPrefix(trie: ConcurrentRadixTree[String /*y_i*/ ], input: String): Option[Rule] = {
    val searchResult: SearchResult = trie.searchTree(input)
    val value = searchResult.nodeFound.getValue.asInstanceOf[String]
    val charsMatched = searchResult.charsMatched
    // Exact match - at the end of an edge, and also has a value - which means it's a complete rule and not just some common prefix of rules.
    if (
      (searchResult.classification == Classification.INCOMPLETE_MATCH_TO_END_OF_EDGE ||
        searchResult.classification == Classification.EXACT_MATCH) &&
        value != null) {
      assert(searchResult.nodeFound.getOutgoingEdges.isEmpty, "Prefix Substitution invariant violated: Patterns can't be prefixes of each other.")
      // Because it's an exact match, we can just take the `charsMatched` first characters of the input that matched to get the key.
      Some(Rule(input.take(charsMatched), value))
    } else {
      None
    }
  }

  def isFinal(trie: ConcurrentRadixTree[String /*y_i*/ ], input: String): Boolean =
    getPrefix(trie, input) == None

  def applyRule(rule: Rule, s: String): String = {
    assert(s.startsWith(rule.pattern))
    rule.replacement ++ s.drop(rule.pattern.length)
  }

  // TODO: This code is shit. Write the good impl, improve code.
  // returns the normal form.
  def naivePreConsistency(_rules: List[Rule]): Option[ConcurrentRadixTree[String]] = {
    val allRules: ConcurrentRadixTree[String] = new ConcurrentRadixTree[String](new SmartArrayBasedNodeFactory)
    val finalRules: ConcurrentRadixTree[String] = new ConcurrentRadixTree[String](new SmartArrayBasedNodeFactory)
    _rules.foreach {
      case Rule(pattern, replacement) => allRules.put(pattern, replacement)
    }
    _rules.filter(rule => isFinal(allRules, rule.replacement)).foreach {
      case Rule(pattern, replacement) => finalRules.put(pattern, replacement)
    }

    while (!allRules.forall(rule => isFinal(allRules, rule.getValue))) {
      val finallyApplicableRules: List[(Rule, Rule)] = allRules.toList.flatMap {
        case KeyValuePair(pattern, replacement) => getPrefix(finalRules, replacement).map(applicableRule => (Rule(pattern, replacement), applicableRule))
      }
      if (finallyApplicableRules.isEmpty) return None
      //   applicant     applicable rule
      for ((Rule(xi, yi), applicableRule) <- finallyApplicableRules) {
        val newYi = applyRule(applicableRule, yi)
        allRules.put(xi, newYi)
        if (isFinal(allRules, newYi))
          finalRules.put(xi, newYi)
      }
    }

    Some(finalRules)
  }

  println(naivePreConsistency(List(Rule("ab", "afb"), Rule("ad", "abc"))))
  // Expected: Rules: ab -> afb, ad -> afbc
  // Explanation: abc -> afbc
}

object KeyValuePair {
  def unapply[V](kv: KeyValuePair[V]): Option[(String, V)] = Some((kv.getKey.toString, kv.getValue))
}
