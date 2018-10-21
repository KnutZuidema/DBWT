# Dossier

## Inhaltsverzeichniss
* [Meilenstein 1](#meilenstein-1)
  * [Frage 1](#frage-1)
  * [Frage 2](#frage-2)
  * [Frage 3](#frage-3)

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

Welche Attribute für Elemente solcher
Drop-Down/Auswahllisten erscheinen
Ihnen außerdem noch nutzbringend?

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