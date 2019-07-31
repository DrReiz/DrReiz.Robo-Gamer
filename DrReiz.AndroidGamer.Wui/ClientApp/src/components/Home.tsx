import React, { Component } from 'react';
import { Button } from 'reactstrap';
import axios from 'axios'

export class Home extends Component {
  static displayName = Home.name;

  game = 'empire';

  captureHandle = async() => {
    const response = axios.post(`api/droid/${this.game}/capture`);
  }

  render() {
    return (
      <div>
        <div>{this.game}</div>
        <Button onClick={this.captureHandle}>Capture</Button>
      </div>
    );
  }
}
