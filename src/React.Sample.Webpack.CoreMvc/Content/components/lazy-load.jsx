import React, { Fragment, useState, useEffect } from 'react';
import { Helmet } from 'react-helmet';

export function LazyLoadDemo() {
	const [lazySelect, setLazySelect] = useState();
	const [framework, setFramework] = useState();
	useEffect(() => {
		import('react-select').then(component => {
			setLazySelect({ Component: component.default });
		});
	}, []);
	return (
		<Fragment>
			<Helmet>
				<title>ReactJS.NET Demos | Lazy loading</title>
			</Helmet>
			{lazySelect && lazySelect.Component ? (
				<lazySelect.Component
					defaultValue={'React'}
					isMulti
					options={['React', 'Vue', 'Svelte', 'Ember', 'Angular'].map(x => ({ value: x, label: x }))}
					className="basic-multi-select"
					classNamePrefix="select"
					onChange={setFramework}
				/>
			) : 'Loading'}
			<div>{framework ? 'Pick a framework' : null}</div>
		</Fragment>
	);
}
