# FSRAudioSwitcher

![FSRAudioSwitcher demo animation)[/demo/demo-animation.gif]

picture this: you have two separate audio output devices on your computer - speakers and a pair of headphones. modern audio drivers allow you to attach these two devices to different audio ports on your computer (back plate and front i/o) and have them show up as separate, selectable devices in windows.

that in turn makes it possible to switch between these devices via software - by clicking the loudspeaker icon in the windows tray bar and selecting the one you'd like to set as the default output device.

now imagine that you don't have to do these three clicks. that's where this piece of software, along with a bit of hardware, comes into play.

you will need:

1. a windows computer with the aforementioned driver settings
2. a headphone stand that you hang your headphones on whenever you're not using them
3. an arduino uno-compatible microcontroller board (or, if you rewrite that part, any)
4. a force sensitive resistor (e.g. FSR-406)
5. a few wires, another resistor and a usb cable

the idea is to:

1. attach the FSR to the headphone stand, so that whenever your headphones weigh down on it, there is a measurable change in resistance
2. measure this resistance with a small circuit on the microcontroller
3. relay this information via the usb cable to the computer
4. have FSRAudioSwitcher switch the devices whenever the measured resistance goes under or over a certain threshold.

# quick setup:

1. assemble the hardware
2. connect it to the computer
3. program the microcontroller
4. start up FSRAudioSwitcher
5. right-click on the tray icon
6. select the devices to switch between, filling the roles of speakers and headphones
7. select the COM port that represents the microcontroller
8. check the resistance measurements when your headphones are resting on the FSR. pick something slightly higher as the threshold.
9. done

# software components

this repository consists of two components; the FSRAudioSwitcher which runs on your computer and speaks to the microcontroller via a COM port, and a small program that your arduino is programmed with to periodically take measurements of the resistor to supply the computer with.

## FSRAudioSwitcher

this piece of software includes the windows-side logic to control the audio device and read measurements sent from the microcontroller board.

FSRAudioSwitcher will display a tray icon from which you can control its settings as well as view the current resistance measurement.

a left-click will dis- or enable the switching functionality.

## Arduino code

the included .ino file contains some simple logic for calculating the current resistance exhibited by the FSR, based on a voltage divider circuit. this data is measured every 200ms and relayed via serial connection to the host computer, where it is read by FSRAudioSwitcher.

Depending on your hardware setup, you will need to change some of the variables.

1. `analogPin` is the pin on the microcontroller that will do the analog-to-digital conversion and let the program read the voltage
2. `r1` is the resistor opposite the FSR, required to calculate the latter's resistance
3. `vin` is the voltage used on the circuit

note that you can also change the delay between measurements, but be advised that it should remain under the read timeout set in FSRAudioSwitcher (2000ms at the time of writing). 200ms are nice and responsive and still leave the microcontroller sleeping most of the time. additionally, the serial port baud rate must be identical to the one set in FSRAudioSwitcher.

# hardware

here's a quick schematic on how to connect the voltage divider circuit. ports to the left connect to the arduino.

![FSR/arduino circuit](/demo/circuit.png)

for the FSR, i used an FSR-406 off amazon. my microcontroller is a cheap 6€ arduino uno clone off ebay. it is connected via usb to the computer.
