import sqlite3
import json
from nltk.corpus import wordnet as wn

def fetch_wordnet_data():
    wordnet_data = []

    for synset in wn.all_synsets():

        for lemma in synset.lemmas():
            word = lemma.name()
            pos = synset.pos()

            if word in synset.name() and pos in ['a', 's', 'n', 'v']:
                definition = synset.definition()
                examples = [example for example in synset.examples() if word in example]
                similar_words = [similar_word for similar_word in synset.lemma_names() if similar_word != word]
                category = synset.lexname()

                # Convert lists to JSON strings
                examples_str = json.dumps(examples)
                similar_words_str = json.dumps(similar_words)

                wordnet_data.append((word, pos, definition, examples_str, similar_words_str, category))

    return wordnet_data

def create_database():
    conn = sqlite3.connect('Assets/Databases/words.db')
    cursor = conn.cursor()

    cursor.execute('''CREATE TABLE IF NOT EXISTS wordnet_data
                    (Word TEXT, POS TEXT, Definition TEXT, Examples TEXT, Similar_words TEXT, Category TEXT)''')

    data = fetch_wordnet_data()

    cursor.executemany('INSERT INTO wordnet_data VALUES (?, ?, ?, ?, ?, ?)', data)

    conn.commit()
    conn.close()

if __name__ == "__main__":
    create_database()
