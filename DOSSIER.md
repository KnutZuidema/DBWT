# Dossier

## Inhaltsverzeichniss
* [Meilenstein 1](#meilenstein-1)
  * [Frage 1](#frage-1)
  * [Frage 2](#frage-2)
  * [Frage 3](#frage-3)
* [Meilenstein 2](#meilenstein-2)
  * [Frage 4](#frage-4)
  * [Frage 5](#frage-5)

## Meilenstein 1

### Frage 1

Welche Möglichkeit kennen Sie, ein Drop-Down-Element
in einem HTML-Formular anzubieten? Suchen Sie nach
Möglichkeiten, dieses Drop-Down-Element mit Ebenen
zu strukturieren.

### Antwort

Ein Dropdown-Element kann mit dem `<select>` Element erzeugt werden.
Die verschiedenen Auswahlmoeglichkeiten koennen mit `<option>` Elementen hinzugefuegt werden.  
Um die Auswahl zu strukturieren koennen `<optgroup>` Elemente verwendet werde.

```html
<select>
    <optgroup label="Group 1">
        <option>Option 1</option>
        <option>Option 2</option>
    </optgroup>
    <optgroup label="Group 2">
        <option>Option 1</option>
        <option>Option 2</option>
    </optgroup>
</select>
```

### Frage 2

Wie können Sie Elemente in dieser Liste zwar anzeigen,
aber als nicht auswählbar definieren?

### Antwort

Um Elemente als nicht auswaehlbar zu definieren, kann dem `<option>` Element das Attribut
`disabled` zugewiesen werden.

```html
<select>
    <option>Option</option>
    <option disabled>Option disabled</option>
</select>
```

### Frage 3

Welche Attribute für Elemente solcher Drop-Down/Auswahllisten erscheinen Ihnen
außerdem noch nutzbringend?

### Antwort

Das `<select>` Element hat folgende moegliche Attribute (zusaetzlich zu den globalen Attributen):

Attribut|Wert|Anwendung
:---:|---|---
`autofocus`|none|Das `<select>` wird automatisch ausgewaehlt wenn die Seite geladen wird
`form`|form ID|Gibt an zu welchem `<form>` das `<select>` Element gehoert
`multiple`|none|Mehr als eine Option koennen ausgewaehlt werden
`required`|none|Gibt an, dass ein Wert gewaehlt werden muss um das `<form>` Element abzuschicken.
`size`|number|Wie viele Options angezeigt werden

Das `<option>` Element hat ebenfalls zusaetzliche moegliche Attribute:

Attribut|Wert|Anwendung
:---:|---|---
`label`|text|Ein kurzes Label fuer eine Option
`selected`|none|Option wird beim Laden der Seite ausgewaehlt
`value`|text|Wert der uebermittelt werden soll

## Meilenstein 2

### Frage 4

Welcher Datentyp eignet sich um festgelegte Werte in einem Feld anzugeben?

### Antwort

Man kann entweder ein `enum` verwenden oder einen neuen `table` anlegen und den festzulegenden
Wert als `foreign key` verwenden

### Frage 5

Wie werden die binären Relationstypen abgebildet?

### Antwort

Relationstyp|Abbildung
:---:|---
1:1|`foreign key` mit `unique` constraint
1:M|`foreign key` in `M` Menge
N:M|`table` mit `foreign key` zu beiden Mengen

### Frage 6

Was ist der Unterschied zwischen einem `table constraint` und einem `column constraint` und wann
sollte welcher benutzt werden?

### Antwort

Ein `table constraint` wird bei jedem Update im `table` geprüft, ein `column constraint` wird
nur grprüft, wenn ein Wert in der Spalte geändert wird.  
`table constraint`s machen Sinn falls ein `check` von mehr als einem Wert abhängen.

### Frage 7

Welche `constraint`s dienen welchem Zweck in MariaDB?

### Antwort

##### Foreign Key

Stellt sicher, dass der Wert in dem angegebenen `table` existiert.

##### Check

Überprüft, ob das angegebene Statement `true` zurückgibt

### Frage 8

Wie kann der Aufzählungstyp `enum` in anderen SQL-Dialekten implementiert werden?

### Antwort

```sql
create table enumeration(
  enum varchar(4) not null check (enum in ('ET', 'INF', 'WI', 'MCD'))
)
```

### Frage 9

Was bewirkt das Semikolon am Ende einer Zeile?

### Antwort

Das Semikolon schließt eine Anweisung ab

## Meilenstein 3

### Frage 10

Weshalb wird ein Cookie gesetzt nachdem einer Session Werte zugewiesen werden?

### Antwort

Es wird ein Session-Cookie gesetzt womit eine Session einem User zugewiesen werden kann

### Frage 11

Was passiert auf der Serverseite wenn ein Cookie geloescht wird und ein weiterer request gesendet wird?

### Antwort

Es wird eine neue Session gestartet

### Frage 12

Wie kann eine Anmeldung ohne Cookies erfolgen?

### Antwort

Beispielsweise per IP-Tracking oder Fingerprinting, dies ist jedoch wesentlich komplexer byw. unsicherer als Cookies

### Frage 13

Wie kann der Preis eines Produktes ueber eine `stored procedure` abgefragt werden?

### Antwort

siehe Code

### Frage 14

Was bedeutet der erste Abschnit des Hash-Codes?

### Antwort

Dieser gibt den verwendeted Algorithmus, die Iterationen sowie die Hash-Bytes an.